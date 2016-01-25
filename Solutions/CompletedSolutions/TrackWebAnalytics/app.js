//===================================================================
//=																	=
//= Copyright (c) 2015 IMS Health Incorporated. All rights reserved.=
//=																	=
//===================================================================
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
		
		$('#firstname').css('border-color','#D9D9D9');
		$('#lastname').css('border-color','#D9D9D9');
		$('#email').css('border-color','#D9D9D9');
		$('#zip').css('border-color','#D9D9D9');
		$('#state').css('border-color','#D9D9D9');

		$('#submit_btn').prop("disabled", true);
		$('#submit_btn').prop("value","Registering");

		//$('#submit_btn').css('background','#3cb0fd');
		$('#submit_btn').removeClass('btn');
		$('#submit_btn').addClass('disabledbutton');
		
		// clear any messages.
		$(formMessages).removeClass('success');
		$(formMessages).removeClass('error');
		$(formMessages).text('');
		// Submit the form using AJAX.
		$.ajax({
			type: 'POST',
			url: $(form).attr('action'),
			data: formData
		})
		.done(function(response) {
			
			$('#submit_btn').prop("disabled", false);
			$('#submit_btn').removeClass('disabledbutton');
			$('#submit_btn').addClass('btn');
			$('#submit_btn').prop("value","Register");
			
			// Make sure that the formMessages div has the 'success' class.
			$(formMessages).removeClass('error');
			$(formMessages).addClass('success');

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
			$('#option').prop("checked", false);
		})
		.fail(function(data) {
	
			$('#submit_btn').prop("disabled", false);
			$('#submit_btn').removeClass('disabledbutton');
			$('#submit_btn').addClass('btn');
			$('#submit_btn').prop("value","Register");
			
			// Make sure that the formMessages div has the 'error' class.
			$(formMessages).removeClass('success');
			$(formMessages).addClass('error');		
			// Set the message text and red borders.
			if (data.responseText !== '') {
				
				//var message = data.responseText;
				if (data.responseText.indexOf("First name is empty") > -1){
					$('#firstname').css('border-color','red');
				}
				if (data.responseText.indexOf("Last name is empty") > -1){
					$('#lastname').css('border-color','red');
				}
				if (data.responseText.indexOf("Email address is invalid or empty") > -1){
					$('#email').css('border-color','red');
				}
				if (data.responseText.indexOf("Invalid U.S. zip code") > -1){
					$('#zip').css('border-color','red');
				}
				if (data.responseText.indexOf("Invalid U.S state") > -1){
					$('#state').css('border-color','red');
				}
							
				$(formMessages).text(data.responseText);
			
			} else {
				$(formMessages).text('An unknown error occured.');
			}
		});

	});

});
