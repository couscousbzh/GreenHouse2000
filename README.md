# GreenHouse2000
Green House 2000


"Parce que l'an 2000 c'est le futur - Nabilla 2015"

Green House 2000 est le nom du projet qui concerne l'automatisation de ma serre. J'ai pris un peu de retard car la saison des tomates est plus qu'avancée... (certainement terminée quand j'aurais fini cet article). Mais bon, j'ai eu le temps de construire une serre (3m par 5m) avec du matos de récup. Et mes tomates poussent déjà super bien en mode "manuel".

J'ai un petit netduino 2 plus d'apprentissage que je souhaite utiliser à bon escient. Ce modèle est un "dérivé" d'Arduino. Il est spécialement concu pour permettre au développeur Microsoft .net C sharp que je suis de pouvoir utiliser son langage préféré grâce au Micro Framework embarqué sur la carte. C'est mon premier achat "IoT". Si je me souvient bien c'est parce qu'il avait un connecteur Ethernet, ce qui n'était pas commun à l'époque. Il dispose en plus d'une carte mini SD. Sinon pour le reste, on a quasiment un Arduino classique. Spec ici : http://www.netduino.com/netduinoplus2/specs.htm

Donc voila, l'idée est d'utiliser le netduino comme centrale pour surveiller ma serre (température, humidité, luminosité, débit eau) et pouvoir piloter un arrosage automatique (voir contrôlé à distance).

Pour cela j'ai acheté 2 capteurs :
- DHT22 Sensor :  capteur de température et d'humidité en 1 seul composant
- un capteur de luminosité.

J'ai aussi acheté un écran LCD 4 lignes.  Pour afficher en live mes données.



Dans un premier temps je me suis concentrer sur l'acquisition de données. Après quelques recherches et test sur un board de prototypage, j'ai enfin pu enregistrer quelques données et les afficher sur mon écran LCD.


Ensuite dans un second temps j'ai travaillé sur l'envoi de ces données sur le net. Pour cela j'ai utilisé un web service spécialisé dans l'IoT ; thingspeak. En deux trois mouvements, j'ai configuré un compte et grâce à une simple clé et quelques ligne de code, j'ai pu envoyé mes données sur le net. Et hop des courbes : https://thingspeak.com/channels/47907




Troisième étape, consolider mon code et pouvoir debuger. J'ai constaté que mon code plantait (pendant mes vacances) et qu'il était pas simple de savoir ce qu'il se passait sur le netduino, surtout déconnecté (et donc pas en mode debug). J'ai donc pensé à un système de log en deux parties:
- log local, enregistré sur la SD Card. Donc c'est peu pratique car il faut prendre la mini SD et la lire sur un PC puis la remettre.
- log distant, j'ai utilisé logentries qui est pas mal



Donc je peux suivre à distance mes courbes de température/humidité/luminosité. Si je constate un plantage, je peux regarder online les logs. Si jamais ces logs ne m'informe pas assez, je vais voir sur la carte SD. Normalement on est bon la...

Phase 2 : le contrôle d'une électrovanne : l'arrosage automatique. Prochainement....