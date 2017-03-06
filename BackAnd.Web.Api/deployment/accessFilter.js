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

    //This action happens after accessing the system with an OAuth2 access token.
    //Return results of "allow" or "deny" to override the Backand OAuth2 access token approval and provide a denied message
    return { "result": "allow", "message": "" };
}