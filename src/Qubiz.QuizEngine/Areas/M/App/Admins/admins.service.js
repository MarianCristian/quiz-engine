﻿(function () {
    'use strict';

    angular
		.module('quizEngineMaterial')
		.service('adminsService', adminsService);

    adminsService.$inject = ['$http', '$q'];

    function adminsService($http, $q) {
        this.getAllAdmins = getAllAdmins;
        this.addAdmin = addAdmin;
        this.deleteAdmin = deleteAdmin;
        this.editAdmin = editAdmin;
        this.getById = getById;

        function getAllAdmins() {
            return $http({
                method: 'GET',
                url: 'api/NewAdmin/GetAdmins'
            });
        }

        function addAdmin(admin) {
            return $http({
                method: 'POST',
                url: 'api/NewAdmin/AddAdmin',
                data: admin
            });
        }

        function editAdmin(admin) {
            return $http({
                method: 'PUT',
                url: 'api/NewAdmin/UpdateAdmin',
                data: admin
            });
        }

        function getById(id) {
            return $http({
                method: 'GET',
                url: 'api/NewAdmin/GetAdmin/' + id
            });
        }

        function deleteAdmin(id) {
            return $http.delete('api/NewAdmin/DeleteAdmin/' + id);
        }
    }
})()