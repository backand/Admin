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
}