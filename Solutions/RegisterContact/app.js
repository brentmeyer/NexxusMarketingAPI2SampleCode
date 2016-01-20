//=========================================================================
//=	default.htm - ajax processing of form data received from default.htm. =
//= Copyright (c) 2015 IMS Health Incorporated. All rights reserved.      =
//=																	                                      =
//=========================================================================

$(function() {

	// Get the form.
	var form = $('#ajax-contact');

	// Get the messages div.
	var formMessages = $('#form-messages');

	// Set up an event listener for the contact form.
	$(form).submit(function(e) {
		// Stop the browser from submitting the form.
		e.preventDefault();

		// Serialize the form data.
		var formData = $(form).serialize();

		// Remove any previously displayed error fields
		$('#form-firstname').removeClass('has-error');
		$('#form-lastname').removeClass('has-error');
		$('#form-email').removeClass('has-error');
		$('#form-zip').removeClass('has-error');
		$('#form-state').removeClass('has-error');

		//Disable fields while processing
		$('#firstname').prop("disabled",true);
		$('#lastname').prop("disabled",true);
		$('#email').prop("disabled",true);
		$('#address1').prop("disabled",true);
		$('#address2').prop("disabled",true);
		$('#city').prop("disabled",true);
		$('#state').prop("disabled",true);
		$('#zip').prop("disabled",true);
		$('#country').prop("disabled",true);
		$('#option').prop("disabled",true);
		$('#submit_btn').prop("disabled", true);
		document.getElementById('submit_btn').innerHTML = 'Registering';

		$(formMessages).text('');
		// Submit the form using AJAX.
		$.ajax({
			type: 'POST',
			url: $(form).attr('action'),
			data: formData
		})
		// Successfully registered the contact
		.done(function(response) {

			$('#submit_btn').prop("disabled", false);
			//$('#submit_btn').removeClass('disabledbutton');
			//$('#submit_btn').addClass('btn');
			document.getElementById('submit_btn').innerHTML = 'Register';

			$('#firstname').prop("disabled",false);
			$('#lastname').prop("disabled",false);
			$('#email').prop("disabled",false);
			$('#address1').prop("disabled",false);
			$('#address2').prop("disabled",false);
			$('#city').prop("disabled",false);
			$('#state').prop("disabled",false);
			$('#zip').prop("disabled",false);
			$('#country').prop("disabled",false);
			$('#option').prop("disabled",false);

			// Make sure that the formMessages div has the 'alert-success' class.
			$(formMessages).removeClass('alert alert-danger');
			$(formMessages).addClass('alert alert-success');
			// Set the message text.
			$(formMessages).text(response);

			// Clear the form.
			$('#firstname').val('');
			$('#lastname').val('');
			$('#email').val('');
			$('#address1').val('');
			$('#address2').val('');
			$('#city').val('');
			$('#state').val('');
			$('#zip').val('');
			$('#country').val('');
			$('#option').prop("checked", true);

			$('#firstname').prop('placeholder','Required');
			$('#lastname').prop('placeholder','Required');
			$('#email').prop('placeholder','Required');
			$('#address1').prop('placeholder','');
			$('#address2').prop('placeholder','');
			$('#city').prop('placeholder','');
			$('#state').prop('placeholder','');
			$('#zip').prop('placeholder','');
			$('#country').prop('placeholder','');

		})
		// Failed to register contact
		.fail(function(data) {
			var fieldError=0;
			$('#firstname').prop("disabled",false);
			$('#lastname').prop("disabled",false);
			$('#email').prop("disabled",false);
			$('#address1').prop("disabled",false);
			$('#address2').prop("disabled",false);
			$('#city').prop("disabled",false);
			$('#state').prop("disabled",false);
			$('#zip').prop("disabled",false);
			$('#country').prop("disabled",false);
			$('#option').prop("disabled",false);

			$('#submit_btn').prop("disabled", false);
			document.getElementById('submit_btn').innerHTML = 'Register';

			// Make sure that the formMessages div has the 'alert-danger' class.
			$(formMessages).removeClass('alert alert-success');
			$(formMessages).addClass('alert alert-danger');

			// Set the error text based on responseText
			if (data.responseText !== '') {
				if (data.responseText.indexOf("First name is empty") > -1){
					$('#form-firstname').addClass('has-error');
					$('#firstname').prop('placeholder','First name is empty');
					fieldError=1;
				}
				if (data.responseText.indexOf("Last name is empty") > -1){
					$('#form-lastname').addClass('has-error');
					$('#lastname').prop('placeholder','Last name is empty');
					fieldError=1;
				}
				if (data.responseText.indexOf("Email address is invalid") > -1){
					var emailedit=document.getElementById('email').value
					$('#email').prop('placeholder', 'Email address is invalid: ' + emailedit);
					$('#email').val('');
					$('#form-email').addClass('has-error');
					fieldError=1;
				}
				if (data.responseText.indexOf("Email address is empty") > -1){
					$('#email').prop('placeholder', 'Email address is empty');
					$('#email').val('');
					$('#form-email').addClass('has-error');
					fieldError=1;
				}
				if (data.responseText.indexOf("Invalid U.S. zip code") > -1){
					$('#zip').prop('placeholder','Invalid U.S. zip code');
					$('#form-zip').addClass('has-error');
					fieldError=1;
				}
				if (data.responseText.indexOf("Invalid U.S state") > -1){
					$('#state').prop('placeholder','Invalid U.S state');
					$('#form-state').addClass('has-error');
					fieldError=1;
				}
			  if (fieldError === 1){
					$(formMessages).text("There was a problem. Please check the hi-lighted fields.");
				}
				else{ //Probably authentication or url. Check php log file.
					$(formMessages).text(data.responseText);
				}
			} else {
				$(formMessages).text('An unknown error occured.');
			}
		});
	});
});
