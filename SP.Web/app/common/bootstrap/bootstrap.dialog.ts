import angular from 'angular';

    class modalDialog {
        $modal:any;
        constructor($modal, $templateCache){
            const id = 'modalDialog.tpl.html';
            this.$modal = $modal;
            if (!$templateCache.get(id)){
                $templateCache.put(id , 
`                    <div>
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true" data-ng-click="cancel()">&times;</button>
                            <h3>{{title}}</h3>
                        </div>
                        <div class="modal-body">
                            <p>{{message}}</p>
                        </div>
                        <div class="modal-footer">
                            <button class="btn btn-primary" data-ng-click="ok()">{{okText}}</button>
                            <button class="btn btn-info" data-ng-click="cancel()">{{cancelText}}</button>
                        </div>
                    </div>`);
            }
        }

        deleteDialog(itemName) {
            var title = 'Confirm Delete';
            itemName = itemName || 'item';
            var msg = 'Delete ' + itemName + '?';

            return this.confirmationDialog(title, msg);
        }

        confirmationDialog(title: string, msg: string, okText?:string, cancelText?:string) {
            var modalOptions = {
                templateUrl: 'modalDialog.tpl.html',
                controller: ['$scope', '$modalInstance', 'options',
                    function ($scope, $modalInstance, options) {
                        $scope.title = options.title || 'Title';
                        $scope.message = options.message || '';
                        $scope.okText = options.okText || 'OK';
                        $scope.cancelText = options.cancelText || 'Cancel';
                        $scope.ok = function () { $modalInstance.close('ok'); };
                        $scope.cancel = function () { $modalInstance.dismiss('cancel'); };
                    }],
                keyboard: true,
                resolve: {
                    options: function () {
                        return {
                            title: title,
                            message: msg,
                            okText: okText,
                            cancelText: cancelText
                        };
                    }
                }
            };
            return this.$modal.open(modalOptions).result; 
        }
    }

export default angular.module('common.bootstrap', ['ui.bootstrap'])
    .service('bootstrap.dialog', ['$modal', '$templateCache', modalDialog])
    .name;

