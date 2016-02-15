# EventsApp
a web app for managing events. MVC, Entity Framework Code First, SQL, Dynamic bundles 


Modellen vi använder är ASP .NET MVC. För datahantering använder vi Entity Framework och Code First för databas uppdatering. Vi använder Dynamic Bundles för hantering av vår styling components såsom html, css, javascript, jquery, bootstrap.

Första sidan av event app -- Log-in och register knapparna syns när man inte är inloggad. Förutom det är det är bara publika events som syns på kartan och eventlistan. Vi använder ASP .NET Identity för medlemhantering och OWIN for autentisering. 
Toggle-funktion på listan finns för att kunna se event description och details-link. Vi använder Google API till vår karta. 
 
Front sida --- när man är inloggad. ”Hello USERNAME” länk öppnar användarens sida, som syns på nästa bild.
”Create Event” är en länk som öppnar en ny sida som innehåller en form för att kunna skapa ett event.
 
Användarens sida. Här kan man se alla events han har skapat. Inloggade person kan se invitations på hans sida där han kan ”accept” eller ”decline” ett event.
 
Event details sida . Här ser man mer detaljer om eventet såsom länk till eventets ägare användare sida, event description, address, tid, position på kartan och inbjudna gäster. 
Inloggade person som skapade eventet kan se några funktioner för att kunna bjuda in folk till sitt event. Det går att bjuda folk via username eller via invitation länk. Funktionen för att kunna ta bort sitt event också finns eller redigera. Edit-knappen öppnar en ny sida där formen för redigering finns.

Laurene Suralta - "Ebony"
Lars Woxberg - "Ivory"
