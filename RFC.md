## RFC for the application "Play board game"
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

### Board games
Board game is described by:
* Title - mandatory
* Users - users who have this game on their shelves 

It is possible to add/edit board game by entering the title. Only administrator can add/edit/delete the board game. It is possible to delete the board game which is not connected to any resource in the application.

User can select which board games posses. Each user can see his/her shelf.

### Meeting
User can organize the meeting with the goard games. In order to plan the meeting the user has to provide:
* When (date and time) - mandatory
* Where
* Who - mandatory
* Which games
* Notes

The application informs the user if there are some conflicts about: when and who.
Who - it is possible to enter email addresses to invite people who are not users.
