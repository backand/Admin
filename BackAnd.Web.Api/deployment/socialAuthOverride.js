/* globals
$http - Service for AJAX calls 
CONSTS - CONSTS.apiUrl for Backands API URL
Config - Global Configuration
*/
'use strict';
function backandCallback(userInput, dbRow, parameters, userProfile) {

    //Example for SSO in OAuth 2.0 standard
    //$http({"method":"POST","url":"http://www.mydomain.com/api/token", "data":"grant_type=password&username=" + userInput.username + "&password=" + userInput.password, "headers":{"Content-Type":"application/x-www-form-urlencoded"}});

    //Return results of "allow" or "deny" to override the Backand auth and provide a denied message
    //Return ignore to ignore this fucntion and use Backand default authentication
    //Return additionalTokenInfo that will be added to backand auth result.
    //You may access this later by using the getUserDetails function of the Backand SDK.
    return { "result": "ignore", "message": "", "additionalTokenInfo": {} };
}