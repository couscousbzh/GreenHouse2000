# GreenHouse2000
Green House 2000


"Parce que l'an 2000 c'est le futur - Nabilla 2015"

Green House 2000 est le nom du projet qui concerne l'automatisation de ma serre. J'ai pris un peu de retard car la saison des tomates est plus qu'avanc�e... (certainement termin�e quand j'aurais fini cet article). Mais bon, j'ai eu le temps de construire une serre (3m par 5m) avec du matos de r�cup. Et mes tomates poussent d�j� super bien en mode "manuel".

J'ai un petit netduino 2 plus d'apprentissage que je souhaite utiliser � bon escient. Ce mod�le est un "d�riv�" d'Arduino. Il est sp�cialement concu pour permettre au d�veloppeur Microsoft .net C sharp que je suis de pouvoir utiliser son langage pr�f�r� gr�ce au Micro Framework embarqu� sur la carte. C'est mon premier achat "IoT". Si je me souvient bien c'est parce qu'il avait un connecteur Ethernet, ce qui n'�tait pas commun � l'�poque. Il dispose en plus d'une carte mini SD. Sinon pour le reste, on a quasiment un Arduino classique. Spec ici : http://www.netduino.com/netduinoplus2/specs.htm

Donc voila, l'id�e est d'utiliser le netduino comme centrale pour surveiller ma serre (temp�rature, humidit�, luminosit�, d�bit eau) et pouvoir piloter un arrosage automatique (voir contr�l� � distance).

Pour cela j'ai achet� 2 capteurs :
- DHT22 Sensor :  capteur de temp�rature et d'humidit� en 1 seul composant
- un capteur de luminosit�.

J'ai aussi achet� un �cran LCD 4 lignes.  Pour afficher en live mes donn�es.



Dans un premier temps je me suis concentrer sur l'acquisition de donn�es. Apr�s quelques recherches et test sur un board de prototypage, j'ai enfin pu enregistrer quelques donn�es et les afficher sur mon �cran LCD.


Ensuite dans un second temps j'ai travaill� sur l'envoi de ces donn�es sur le net. Pour cela j'ai utilis� un web service sp�cialis� dans l'IoT ; thingspeak. En deux trois mouvements, j'ai configur� un compte et gr�ce � une simple cl� et quelques ligne de code, j'ai pu envoy� mes donn�es sur le net. Et hop des courbes : https://thingspeak.com/channels/47907




Troisi�me �tape, consolider mon code et pouvoir debuger. J'ai constat� que mon code plantait (pendant mes vacances) et qu'il �tait pas simple de savoir ce qu'il se passait sur le netduino, surtout d�connect� (et donc pas en mode debug). J'ai donc pens� � un syst�me de log en deux parties:
- log local, enregistr� sur la SD Card. Donc c'est peu pratique car il faut prendre la mini SD et la lire sur un PC puis la remettre.
- log distant, j'ai utilis� logentries qui est pas mal



Donc je peux suivre � distance mes courbes de temp�rature/humidit�/luminosit�. Si je constate un plantage, je peux regarder online les logs. Si jamais ces logs ne m'informe pas assez, je vais voir sur la carte SD. Normalement on est bon la...

Phase 2 : le contr�le d'une �lectrovanne : l'arrosage automatique. Prochainement....