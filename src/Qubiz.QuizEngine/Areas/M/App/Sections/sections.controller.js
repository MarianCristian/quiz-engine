﻿(function () {
    'use strict'
    angular
        .module('quizEngineMaterial')
        .controller('SectionsController', SectionsController)
    
    SectionsController.$inject = ['SectionsDataService', '$mdDialog'];

    function SectionsController(SectionsDataService, mdDialog) {
        
        var vm = this;
        vm.sections = {};
        vm.getAllSections = getAllSections;
        vm.deleteSection = deleteSection;

        getAllSections();

        function getAllSections() {
            vm.sections = SectionsDataService.getAllSections()
                .then(getSectionsSuccess)
                .catch(errorCallBack);
        }

        function getSectionsSuccess(sections) {
            vm.sections = sections;
        }

        function errorCallBack(errorMsg) {
            console.log('Error message: ' + errorMsg);
        }

        function deleteSection(id) {
            SectionsDataService.deleteSection(id)
                .then(deleteSuccess)
                .catch(errorCallBack);
        }

        function deleteSuccess(response) {
            console.log(response);
            getAllSections();
        }

        // pop up menu
        vm.showConfirm = function (ev, section) {
            var confirm = mdDialog.confirm()

                  .title('Are you sure you want to delete this section?')
                  .textContent('This action cannot be undone.')
                  .targetEvent(ev)
                  .cancel('No')
                  .ok('Yes');

            mdDialog.show(confirm).then(function () {
                deleteSection(section.ID);
                vm.status = 'Section deleted successfuly.';
            }, function () {

                vm.status = 'Deletion aborted.';
            });
        };    
    }
   
})();