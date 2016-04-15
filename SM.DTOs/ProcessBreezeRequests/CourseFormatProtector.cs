using Breeze.BusinessTime;
using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
using LinqKit;
using SM.DataAccess;
using SM.DataAccess.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;

namespace SM.DTOs.ProcessBreezeRequests
{
    internal class CourseFormatProtector : IProcessBreezeRequests
    {
        private readonly IPrincipal _user;
        private readonly MedSimDbContext _context;

        public CourseFormatProtector(IPrincipal user, MedSimDbContext context)
        {
            _user = user;
            _context = context;
        }

        public void Process(Dictionary<Type, List<EntityInfo>> saveMap)
        {
            IEnumerable<EFEntityError> errors = new EFEntityError[0];

            List<EntityInfo> currentInfos;
            if (saveMap.TryGetValue(typeof(CourseFormat), out currentInfos))
            {
                errors = errors.Concat(GetCourseFormatErrors(currentInfos));
            }

            if (saveMap.TryGetValue(typeof(CourseType), out currentInfos))
            {
                errors = errors.Concat(GetParticipantErrors(currentInfos));
            }

            if (errors.Any())
            {
                throw new EntityErrorsException(errors);
            }
        }

        IEnumerable<EFEntityError> GetCourseFormatErrors(List<EntityInfo> currentInfos)
        {
            var cfs = TypedEntityinfo<CourseFormat>.GetTyped(currentInfos);

            //multiple individual queries may be the way to go here
            var pred = cfs.Aggregate(PredicateBuilder.False<CourseFormat>(), (prev, cur) => prev.Or(
                c => cur.Entity.Id == c.Id && 
                    c.CourseTypeId != cur.Entity.CourseTypeId));
            if (_context.CourseFormats.Any(pred.Compile()))
            {
                throw new InvalidDataException();
            }

            var ids = cfs.Select(cf => cf.Entity.Id).ToList();
            var courseTypeIds = cfs.Select(cf => cf.Entity.CourseTypeId);

            var newFormatsForType = (from c in _context.CourseFormats
                                     where courseTypeIds.Contains(c.CourseTypeId) && !ids.Contains(c.Id)
                                     select new { c.Id, c.Description }).ToList();

            newFormatsForType.AddRange(cfs.Select(c => new { c.Entity.Id, c.Entity.Description }));

            return (from c in newFormatsForType
                    group c by c.Description into cg
                    where cg.Count() > 1
                    select cg).SelectMany(i => i)
                    .Where(i => ids.Contains(i.Id))
                    .Select(i => new EFEntityError(cfs.First(ci => ci.Entity.Id == i.Id).Info,
                        "RepeatWithinGroup",
                        string.Format("Each course format description must be unique within course type. [{0}]", i.Description),
                        "Description"));
        }

        IEnumerable<EFEntityError> GetParticipantErrors(List<EntityInfo> currentInfos)
        {
            var ps = TypedEntityinfo<Participant>.GetTyped(currentInfos);

            /* too dificult, and there are exceptions - had been trying to keep drs as drs etc
            var pred = PredicateBuilder.False<ProfessionalRole>();
            foreach (var p in ps)
            {
                object pr;
                if (p.Info.OriginalValuesMap.TryGetValue("ProfessionalRoleId", out pr) && !p.Entity.DefaultProfessionalRoleId.Equals(pr))
                {
                    _context.ProfessionalRoles.;
                }
            }
            */
            List<EFEntityError> returnVar = new List<EFEntityError>();
            foreach (var p in ps)
            {
                var dup = (from u in _context.Users
                           where p.Entity.Id != u.Id &&
                               p.Entity.FullName == u.FullName &&
                               p.Entity.DefaultDepartmentId == u.DefaultDepartmentId
                           select u.DefaultProfessionalRoleId).FirstOrDefault();
                if (dup != default(Guid) 
                    && ((dup == p.Entity.DefaultProfessionalRoleId 
                        || (from r in _context.ProfessionalRoles
                            where (new[] { dup, p.Entity.DefaultProfessionalRoleId}).Contains(r.Id)
                            group r by r.Description into c
                            select c).Count() == 1)))
                { 
                    returnVar.Add(new EFEntityError(p.Info,
                        "DuplicateUser",
                        "2 users with the same name, department and profession",
                        "FullName"));
                }

            }
            return returnVar;

        }

        public class InvalidDataException : Exception
        {
            public InvalidDataException() : base() { }
            public InvalidDataException(string msg) : base(msg) { }
        }

        class TypedEntityinfo<T>
        {
            internal T Entity;
            internal EntityInfo Info;

            internal static IEnumerable<TypedEntityinfo<T>> GetTyped(IEnumerable<EntityInfo> info)
            {
                return info.Select(i => new TypedEntityinfo<T> { Info = i, Entity = (T)i.Entity }).ToList();
            }
        }
    }
}
