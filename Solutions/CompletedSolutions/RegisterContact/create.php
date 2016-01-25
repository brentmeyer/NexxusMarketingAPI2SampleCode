<?php
//========================================================================
//= create.php - SmartMerges contact and creates OptinActivity           =
//= resources for supplied form data.                                    =
//= Copyright (c) 2015 IMS Health Incorporated. All rights reserved.     =
//=                                                                      =
//========================================================================
ini_set("log_errors", 1);
error_log( "Nexxus Marking Registration sample: Starting Registration..." );

//Validation functions
function ValidateZipCode($zipcode) {
    return preg_match('/^[0-9]{5}([- ]?[0-9]{4})?$/', $zipcode);
}
function ValidateState($state) {
    if (preg_match('/^(?:A[KLRZ]|C[AOT]|D[CE]|FL|GA|HI|I[ADLN]|K[SY]|LA|M[ADEINOST]|N[CDEHJMVY]|O[HKR]|PA|RI|S[CD]|T[NX]|UT|V[AT]|W[AIVY])*$/', $state)){
        return true;
    }
    if (preg_match('/^(?:Alabama|Alaska|Arizona|Arkansas|California|Colorado|Connecticut|Delaware|Florida|Georgia|Hawaii|Idaho|Illinois|Indiana|Iowa|Kansas|Kentucky|Louisiana|Maine|Maryland|Massachusetts|Michigan|Minnesota|Mississippi|Missouri|Montana|Nebraska|Nevada|New\sHampshire|New\sJersey|New\sMexico|New\sYork|North\sCarolina|North\sDakota|Ohio|Oklahoma|Oregon|Pennsylvania|Rhode\sIsland|South\sCarolina|South\sDakota|Tennessee|Texas|Utah|Vermont|Virginia|Washington|West\sVirginia|Wisconsin|Wyoming)*$/', $state)){
        return true;
    }

    return false;

}
// General function for creating Nexxus Marketing resources/entities.
function CreateResource($clientObject, $parameters)
{
    $resultArray = Array();
    $resultArray['id'] = 0;
    $resultArray['success'] = false;
    $id=0;
    try {
        $result = $clientObject->Create($parameters);
    }
    catch (Exception $e) {
        error_log( "Nexxus Marketing Registration sample: SOAP call Create failed - ".$e->getMessage() );
        $err= $e.Message;
        throw($e);
    }
    if ($result->CreateResult->BatchCompleted){

        foreach ($result->CreateResult->Resources as $resultResource) // Should be only 1
        {
            if ($resultResource->OperationSucceeded == true){
                $fieldarray=(array)$resultResource->Resource->Field;
                $key=array_search("Id", $fieldarray);
                $id=$fieldarray[$key]->{"_"};
                $resultArray['id'] = $id;
                $resultArray['success'] = true;
            }else{
              error_log( "Nexxus Marketing Registration sample: Couldn't create resource  - $resultResource->ErrorMessage" );
                throw new Exception("$resultResource->ErrorMessage");

            }
        }
    }
    return $resultArray;
}

// General function for Smart Merging resources/entities.
function SmartMergeResource($clientObject, $parameters)
{
    $resultArray = Array();
    $resultArray['id'] = 0;
    $resultArray['success'] = false;
    $id=0;

    try {
        $result = $clientObject->SmartMerge($parameters);
    }
    catch (Exception $e) {
        error_log( "Nexxus Marketing Registration sample: SOAP call SmartMerge failed - ".$e->getMessage() );
        throw $e;
    }

    if ($result->SmartMergeResult->BatchCompleted){

        foreach ($result->SmartMergeResult->Resources as $resultResource) // Should be only 1
        {
            if ($resultResource->OperationSucceeded == true){
                $fieldarray=(array)$resultResource->Resource->Field;
                $key=array_search("Id", $fieldarray);
                $id=$fieldarray[$key]->{"_"};
                $resultArray['id'] = $id;
                $resultArray['success'] = true;
                error_log("Nexxus Marking Registration sample: {$resultResource->UpsertResult} SmartMerge operation for contact {$id}");
             }
            else{
              error_log( "Nexxus Marketing Registration sample: Couldn't smart merge resource  - {$resultResource->ErrorMessage}" );
                throw new Exception("$resultResource->ErrorMessage");
            }
        }
    }
    return $resultArray;
}
//Validates input form.
function ValidateForm()
{
    $validForm = true;
    $zipValid=true;
    $stateValid=true;

    $zipErr="";
    $emailErr="";
    $firstNameErr="";
    $lastNameErr="";
    $stateErr="";
    // Get form information
    $firstName=htmlspecialchars($_POST['firstname']);
    $lastName=htmlspecialchars($_POST['lastname']);
    $email=htmlspecialchars($_POST['email']);
    $address1=htmlspecialchars($_POST['address1']);
    $address2=htmlspecialchars($_POST['address2']);
    $city=htmlspecialchars($_POST['city']);
    $state=htmlspecialchars($_POST['state']);
    $zip=htmlspecialchars($_POST['zip']);
    $country=htmlspecialchars($_POST['country']);
    $inform=htmlspecialchars($_POST['inform']);

    // Validate fields
    if (empty($firstName)){
        $firstNameErr="First name is empty.";
        $validForm=false;
    }
    if (empty($lastName)){
        $lastNameErr="Last name is empty.";
        $validForm=false;
    }

    if (empty($email)){
        $emailErr="Email address is empty.";
        $validForm=false;
    }
    else{
        if (!filter_var($email, FILTER_VALIDATE_EMAIL)) {
            $emailErr = 'Email address is invalid.';
            $validForm = false;
        }
    }
    //United States zip code and state validation
    if ($country == "United States"){
        if (!empty($zip)){

            $zipValid=ValidateZipCode($zip);

            if (!$zipValid ){
                $zipErr = "Invalid U.S. zip code.";
                $validForm = false;
            }
        }
        if (!empty($state)){
            $stateValid=ValidateState($state);
            if (!$stateValid){
                $stateErr = "Invalid U.S state.";
                $validForm = false;
            }
        }
    }

    if ($validForm==false){
        http_response_code(400);
        echo "There was a problem with the following: $firstNameErr $lastNameErr $emailErr $zipErr $stateErr";
        error_log( "Nexxus Marking Registration sample: Validation problem - $firstNameErr $lastNameErr $emailErr $zipErr $stateErr" );

    }
    return $validForm;
}
//Creates an Activity Resource
function CreateActivity($client,$contactId){
    //Build Activity field opbjects.
    $nameObject= (object)array("_" => "Registration Sample", "Id"=>"Name");
    $contactIdObject = (object)array("_" => $contactId, "Id"=>"ContactId");
    $activityTypeObject = (object)array("_" => "Registration", "Id"=>"ActivityType");
    $activitySubTypeObject = (object)array("_" => "Create", "Id"=>"ActivitySubType");
    $notesObject = (object)array("_" => "Nexxus API call to register contact.", "Id"=>"Notes");
    $statusObject = (object)array("_" => "Completed", "Id"=>"Status");


    // Parameter structure
    $fields = array($nameObject,
            $contactIdObject,
            $activityTypeObject,
            $activitySubTypeObject,
            $notesObject,
            $statusObject);

    $resource = array("Field" => $fields);
    $resources = array("Resource" => $resource);
    $parameters = array("TypeId"=>"Activity",
            "Resources" => $resources);

    // create the Activity.
    try{
        $result = CreateResource($client,$parameters);
    }
    catch(Exeption $e){
        error_log( "Nexxus Marketing Registration sample: Couldn't record activity - ".$e->getMessage());
         throw new Exception("An internal error occurred. However, your contact details and preferences were successfully recorded.");
    }
    return $result;
}

// Creates the SOAP client used to access Nexxus Marketing SOAP operations.
function CreateSoapClient(){
    $details=parse_ini_file("../config.ini");

    //Format for no proxy required.
    $options = array(
    'login' => $details['user'],
    'password' => $details['password']);
/*
    //Format for proxy required.
        $options = array(
    'login' => $details['user'],
    'password' => $details['password'],
    'proxy_host' =>$details['proxy'],
    'proxy_port' => $details['proxyport']
    );

    // Format for proxy requiring password.
    $options = array(
    'login' => $details['user'],
    'password' => $details['password'],
    'proxy_host' =>$details['proxy'],
    'proxy_port' => $details['proxyport'],
    'proxy_login' => $details['proxyuser'],
    'proxy_password' => $details['proxypassword']
);
*/
    $url=$details['url'];
    try{
        $client = new SoapClient($url,$options);
    }
    catch (Exception $e)
    {
        error_log( "Nexxus Marketing Registration sample: Couldn't get SOAP object - ".$e->getMessage());
        throw new Exception("Couldn't reach the remote server to register your details.");
    }
  return $client;
}
// Creates/updates Contact resource instance
function SmartMergeContact($client){
    //Build contact field opbjects.
    $firstNameObject= (object)array("_" => $_POST['firstname'], "Id"=>"FirstName");
    $lastNameObject = (object)array("_" => $_POST['lastname'], "Id"=>"LastName");
    $emailObject = (object)array("_" => $_POST['email'], "Id"=>"Email");
    $address1Object = (object)array("_" => $_POST['address1'], "Id"=>"Address1");
    $address2Object = (object)array("_" => $_POST['address2'], "Id"=>"Address2");
    $cityObject = (object)array("_" => $_POST['city'], "Id"=>"City");
    $stateObject = (object)array("_" => $_POST['state'], "Id"=>"State");
    $zipObject = (object)array("_" => $_POST['zip'], "Id"=>"Zip");
    $countryObject = (object)array("_" => $_POST['country'], "Id"=>"Country");
    $contactTypeObject = (object)array("_" => "Contact", "Id"=>"ContactType");

    // Parameter structure
    $fields = array($firstNameObject,
            $lastNameObject,
            $emailObject,
            $address1Object,
            $address2Object,
            $cityObject,
            $stateObject,
            $zipObject,
            $countryObject,
            $contactTypeObject);

    $resource = array("Field" => $fields);
    $resources = array("Resource" => $resource);
    $parameters = array("TypeId"=>"Contact",
            "Resources" => $resources);

    // SmartMerge the contact.
    try{
    // Use the default merge configuration
        $result = SmartMergeResource($client,$parameters);
    }
    catch(Exception $e){
        error_log( "Nexxus Marketing Registration sample: Couldn't create contact - ".$e->getMessage());
        throw new Exception("Could not register your contact details.");
    }
    return $result;
}
// Setup the optin activity resource parameters
function CreateOptinActivity($client,$contactId){
    $targetEntityId = (object)array("_" => $contactId, "Id"=>"TargetEntityId");

    //Opt in if the form optin check box was selected.
    if ($_POST['inform'] == 'yes'){
        error_log("Nexxus Marking Registration sample: Opted in ".$contactId);
        $stateOptin = (object)array("_" => "In", "Id"=>"State");
        $channel = (object)array("_" => "Email", "Id"=>"Channel");
    }
    else{
        error_log("Nexxus Marking Registration sample: Opted out ".$contactId);
        $stateOptin = (object)array("_" => "Out", "Id"=>"State");
        $channel = (object)array("_" => "Email", "Id"=>"Channel");
    }
    //1 = general communication.
    $topicId = (object)array("_" => "1", "Id"=>"TopicId");
    $optinFields = array($targetEntityId,
        $stateOptin,
        $channel,
        $topicId);

    $optin = array("Field" => $optinFields);
    $resourcesOptin = array("Resource" => $optin);
    $parametersOptin = array("TypeId"=>"OptInActivity",
    "Resources" => $resourcesOptin);

    // Create optin activity call.
    try{
        $resultOptin = CreateResource($client, $parametersOptin);
    }
    catch(Exception $e){
        error_log( "Nexxus Marketing Registration sample: Couldn't create optin preference - ".$e->getMessage());
        throw new Exception("Your contact details are registered, but your email preference couldn't be stored.");
    }
    return $resultOptin;
}

// Only process POST requests.
    if ($_SERVER["REQUEST_METHOD"] == "POST") {
        if(ValidateForm() == true) //No error.
        {
                $client=null;
                $result=null;
            // record contact and optin preferences. If it fails, let the user know.

                try{
                        $client = CreateSoapClient();
                        $result=SmartMergeContact($client);
                    CreateOptinActivity($client,$result["id"]);
                }
                catch (Exception $e){
                    http_response_code(500);
                    echo "There was an issue with your registration, {$_POST['firstname']} - ".$e->getMessage();
                    return false;
              }
                    // record activity. Not essential for recording contact details.
                try{
                        CreateActivity($client,$result["id"]);
                    }
                catch (Exception $e){
                    // Non-fatal errors. Don't bother asking visitor to re-submit.
                        echo $e->getMessage();
                }
                http_response_code(200);
                echo "Contact successfully registered.";
                return true;
            }
    }
    else {
        // Not a POST request, set a 403 (forbidden) response code.
        http_response_code(403);
        echo "There was a problem with your submission, {$_POST['firstname']}, please try again.";
        error_log( "Nexxus Marketing Registration sample: Something other than a POST request received." );
    }
?>
