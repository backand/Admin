/* globals
  $http - service for AJAX calls - $http({method:"GET",url:CONSTS.apiUrl + "/1/objects/yourObject" , headers: {"Authorization":userProfile.token}});
  CONSTS - CONSTS.apiUrl for Backands API URL
*/
'use strict';
function backandCallback(userInput, dbRow, parameters, userProfile) {
	
    if (parameters.sync)
        return {};

    var randomPassword = function (length) {
	    if (!length) length = 10;
	    return Math.random().toString(36).slice(-length);
	}
    if (!parameters.password){
        parameters.password = randomPassword();
    }
	
    var backandUser = {
        password: parameters.password,
        confirmPassword: parameters.password,
        email: userInput.email,
        firstName: userInput.firstName,
        lastName: userInput.lastName,
        parameters: { "sync": true }
    };
    
    // uncomment if you want to debug 
    //console.log(parameters);
	
    var x = $http({method:"POST",url:CONSTS.apiUrl + "1/user" ,data:backandUser, headers: {"Authorization":userProfile.token, "AppName":userProfile.app}});

    // uncomment if you want to return the password and sign in as this user
    //return { password: parameters.password };
    return { };
}