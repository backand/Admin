-- USE `__DB_NAME__`;

/****** Object:  Table `durados_Html`    Script Date: 01/17/2011 11:45:31 ******/


CREATE TABLE `durados_Html`(
	`Name` VARCHAR(50) NOT NULL,
	`Text` LONGTEXT NOT NULL,
PRIMARY KEY  
(
	`Name` ASC
));

INSERT INTO `durados_Html` (`Name`,`Text`) values ('newUserInvitationSubject','Welcome to `Product`');
INSERT INTO `durados_Html` (`Name`,`Text`) values ('newUserInvitationMessage','<div>Hello `User.First Name`,<br><br>Thank you for joining `Product`<br><br>Please click <a href="`AppPath`/Account/LogOn?userid=`Guid`" >here</a> to start<br> If you are not a regidtered Back& user, you will have to register first.<br><br>Sincerely,<br><br>`Product` Team</Div>');


/****** Object:  Table `durados_Html`    Script Date: 07/11/2012 19:29:00 ******/

INSERT `durados_Html` (`Name`, `Text`) VALUES (N'passwordResetConfirmationMessage', N'<div>Hi `FullName`,<br><br><br>We received a request to reset the password for your account.  If you made this request, click  <a href=''`Url`/Account/ChangePassword?id=`Guid`&isReset=true'' >here to reset your password.</a><br> If you didn''t make this request, please ignore this email..<br><br>Cheers, <br><br>The `Product` Team</Div>');
INSERT `durados_Html` (`Name`, `Text`) VALUES (N'passwordResetConfirmationSubject', N'Reset your Backand password.');
INSERT `durados_Html` (`Name`, `Text`) VALUES (N'DefaultPageContent', N'<h1>This is the first page</h1><h2>Place your content here</h2>');

