(function() {
    'use strict';

    var serviceId = 'repository.participant';
    angular.module('app').factory(serviceId,
        ['model', 'repository.abstract', '$q',RepositoryParticipant]);

    function RepositoryParticipant(model, AbstractRepository, $q) {
        var attendeesQuery = breeze.EntityQuery.from('participants');
        var entityName = model.entityNames.participant;
        var orderBy = 'name';

        return {
            create: createRepo // factory function to create the repository
        };

        /* Implementation */
        function createRepo(manager) {
            var base = new AbstractRepository(manager, entityName, serviceId);
            var count = undefined;
            var repo = {
                getAll: getAll,
                getCount: getCount,
                getFilteredCount: getFilteredCount
            };

            return repo;

            function fullNamePredicate(filterValue) {
                return breeze.Predicate
                    .create('name', 'startsWith', filterValue);
            }

            function getAll(forceRemote, page, size, nameFilter) {
                // Only return a page worth of attendees
                var take = size || 20;
                var skip = page ? (page - 1) * size : 0;

                if (base.zStorage.areItemsLoaded('participants') && !forceRemote) {
                    // Get the page of attendees from local cache
                    return $q.when(getByPage());
                }

                // Load all participants to cache via remote query
                return participantsQuery
                    .select('id, name, imageSource')
                    .orderBy(orderBy)
                    .toType(entityName)
                    .using(base.manager).execute()
                    .then(querySucceeded).catch(base.queryFailed);

                function querySucceeded(data) {
                    var participants = base.setIsPartialTrue(data.results);
                    base.zStorage.areItemsLoaded('participants', true);
                    base.log({ msg: 'Retrieved [Participants] from remote data source', data: attendees.length });
                    base.zStorage.save();
                    return getByPage();
                }

                function getByPage() {
                    var predicate = base.predicates.isNotNullo;

                    if (nameFilter) {
                        predicate = predicate.and(fullNamePredicate(nameFilter));
                    }

                    var participants = participantsQuery
                        .where(predicate)
                        .orderBy(orderBy)
                        .take(take).skip(skip)
                        .using(base.manager)
                        .executeLocally();

                    return participants;
                }

            }

            function getCount() {
                if (base.zStorage.areItemsLoaded('participants')) {
                    return $q.when(base.getLocalEntityCount(entityName));
                }
                if (count !== undefined) { return base.$q.when(count); }
                // Attendees aren't loaded and don't have a count yet;
                // ask the server for a count and remember it
                return attendeesQuery.take(0).inlineCount()
                    .using(base.manager).execute()
                    .then(function(data) {
                         return count = data.inlineCount;
                    });
            }

            function getFilteredCount(nameFilter) {
                var predicate = breeze.Predicate
                    .and(base.predicates.isNotNullo)
                    .and(fullNamePredicate(nameFilter));

                var participants = attendeesQuery
                    .where(predicate)
                    .using(base.manager)
                    .executeLocally();

                return participants.length;
            }
        }
    }
})();