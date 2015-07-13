/* globals
  $http - service for AJAX calls - $http({method:"GET",url:CONSTS.apiUrl + "/1/objects/yourObject" , headers: {"Authorization":userProfile.token}});
  CONSTS - CONSTS.apiUrl for Backands API URL
*/
'use strict';
function backandCallback(userInput, dbRow, parameters, userProfile) {
	var validEmail = function(email) 
    {
        var re = /\S+@\S+\.\S+/;
        return re.test(email);
    }
    
    // write your code here
	if (!userInput.email){
        throw new Error("Backand user must have an email.");
    }
    
    if (!validEmail(userInput.email)){
        throw new Error("The email is not valid.");
    }
    if (!userInput.firstName){
        throw new Error("Backand user must have a first name.");
    }
    if (!userInput.lastName){
        throw new Error("Backand user must have a last name.");
    }
    
	var randomPassword = function(length){
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
        lastName: userInput.lastName
    };
        
    console.log(parameters);
	
    var x = $http({method:"POST",url:CONSTS.apiUrl + "1/user" ,data:backandUser, headers: {"Authorization":userProfile.token, "AppName":userProfile.app}});
	return {password:parameters.password};

}