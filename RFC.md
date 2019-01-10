## RFC for the application "Let's play board game"
The main goal of the application is be a tool to organize the meeting with board games and add the information about this meetup: scores and results of the games.

### Roles
There are two roles in the application: administrator and non administrator.

### Users
Only registered users can log in to the application. User can register by using google authorization or by providing:
* Email address - mandatory
* Password - mandatory.
User receives an email if the registration ends with success.
User is described by:
* Email address (login) - mandatory
* Password - mandatory
* First name
* Last name
* Address:
  * Street
  * City
  * Code
  * Country
* Phone number
* Role

User can edit his/her data on user profile page.  
User can change his/her password.  
If the user forgot the password he/she can reset the password by providing email address and then follows the step from mail - link to resetting the password.

### Board games
Board game is described by:
* Title - mandatory
* Users - users who have this game on their shelves 

It is possible to add/edit board game by entering the title. Only administrator can add/edit/delete the board game. It is possible to delete the board game which is not connected to any resource in the application.

User can select which board games posses. Each user can see his/her shelf.

### Meeting
User can organize the meeting with the goard games. In order to add the meeting the user has to provide:
* When (date and time) - mandatory
* Where
* Who - mandatory
* Which games
* Notes
* Is cancelled

The application informs the user if there are some conflicts about: when and who.
Where - there is an option to copy address from user profile.
Who - it is possible to enter email addresses to invite people who are not the users.
Creating new meeting causes that there are sent mails to all invited users and to all invited people who are not the users. Email sent to users contains all information about the meeting. Email sent to people who are not the users contains all information about the meeting and link to registration page (email address entered by default).  
Only organizer (user who added the meeting) can edit/cancel the meeting. There are sent mails to "Who" about all changes of the meeting.
It is not possible to edit cancelled meeting.  
Each user added as "Who" can send the message about the meeting to all the invited users:
* Message - mandatory  
All messages are visible with the date of sending and who sent the message. There is mail sent to all invited people about the message as well.  
Each user can see only the meeting to which he/she is invited.

### Results, scores
It is possible to add the results of the game:
* Meeting
* Game - mandatory
* Number of points
* User
* Description

TODO: reports from results
