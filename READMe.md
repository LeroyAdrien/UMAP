Projet A: Visualisation 3D de UMAP

1) Transformer un CSV en liste de listes -Good
a) Importer le fichier csv 
b) le convertir

2) Convertir chaque liste en une image
a) obtenir une image
b) la scaler, modifier sa position


3) coder UMAP

4) Projeter en 3D chaque image une fois UMAP réalisé 


Projet B: Montrer le passage de 3D vers 2D

1) Points en 3d reliés par des arrêtes et probabilités entre les noeuds indiquées en "hautes" dimensions
2) Points en 3d reliés par des arrêtes et probabilités entre les noeuds indiquées en "basses" dimensions
3) Points en 2D aléatoirement situés qui se déplacent pour correspondre aux probabilités en "basses" dimensions



Structures!!

Projet A:

1) Une classe "Statique" qui lit les csv et les convertit en liste de listes | Un fichier csv -> List<float[n]> avec n la dimension des objets
2) Une classe "Statique" qui prend une liste et renvoie une image | float[n] -> GameObject (Prefab "plan", update toujours tourné vers la caméra)
3) Une classe "Statique" qui fait UMAP | List<float[n]> -> List<float>[n'] avec n' la dimension voulue
4) Une classe qui représente les points | List<float>[n'] + List<GameObject> 

Projet B:

1) Une classe qui représente les arrêtes entre les points "haute" dimension et en "basse" dimension | List<float>[n'] 
3) Une classe qui réprésente des points aléatoires en 2D et fait le lien avec les points en 3D et fait bouger les points en 2D à chaque itération List<float>[n'] + List<GameObject> 


