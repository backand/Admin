/* globals
  $http - Service for AJAX calls 
  CONSTS - CONSTS.apiUrl for Backands API URL
  Config - Global Configuration
  socket - Send realtime database communication
  files - file handler, performs upload and delete of files
  request - the current http request
*/
'use strict';
function backandCallback(userInput, dbRow, parameters, userProfile) {

    var additionalTokenInfo = {};
    //How to retrieve additional information into the user profile example:
    /*
    if (parameters.provider == "facebook") {
        var currency = $http({ "method": "POST", "get": "https://graph.facebook.com/v2.8/me?fields=currency&access_token=" + parameters.providerAccessToken });
        additionalTokenInfo.currency = currency.user_currency;
    }
    */
    //This action happens after social auth approved the user authentication.
    //Return results of "allow" or "deny" to override the Backand social auth approval and provide a denied message
    //Return ignore to ignore this fucntion and use Backand default socail authentication
    //Return additionalTokenInfo that will be added to backand auth result.
    //The additional values must be a flat object with no more than 5 fields and without sub objects due to OAuth 2 and JWT standards.
    //You may access this later by using the getUserDetails function of the Backand SDK or in the userProfile in the action arguments
    return { "result": "allow", "message": "", "additionalTokenInfo": additionalTokenInfo };
}