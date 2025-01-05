*/***************************************************************/

// Alias pour représentation d'un repas (index de nourriture, position X, position Y) ainsi que
// de la souris (état, position X, position Y).
using System.Reflection.Metadata.Ecma335;
using TupleAttributs = (int index, int x, int y);

Random rnd = new Random();        // générateur pour produire les repas
Console.CursorVisible = false;    // ne pas afficher le curseur clignotant dans la console

// Pour sauvegarder les dimensions de la console afin de détecter les redimensionnements.
int hauteurConsole, largeurConsole;

// État de la souris affichée ainsi que sa position dans la console, initialisées à
// l'état normal et positionnée au coin supérieur gauche de la console.
TupleAttributs souris = new TupleAttributs(0, 0, 0);

// Chaînes représentant les différents états possibles de la souris.
string[] typesApparance = {
    "('-')",               // état normal
    "(^-^)",               // état survitaminé
    "(X_X)"                // état malade
};

// Chaînes représentant les différents types de nourriture
string[] typesNourriture = {
    "@@@",                 // remet la souris à son état normal
    "$$$",                 // rend la souris survitaminée
    "###"                  // rend la souris malade
};

// Couleurs à appliquer au repas selon le type de nourriture.
ConsoleColor[] couleurs = { ConsoleColor.Yellow, ConsoleColor.Green, ConsoleColor.Red };

// Attributs du repas : son type de nourriture et sa position dans la console.
TupleAttributs repas;

// Indique quand il faut quitter la partie.
bool quitterPartie;

// Enfin, démarrer la partie.
boucleDeJeu();

/******************************************************************************************
 Programme principal - Gère le jeu jusqu'à ce qu'il se termine.
 ******************************************************************************************/
void boucleDeJeu()
{
    // Initialisation de la partie au démarrage de l'application
    initialiserPartie();

    // On stocke les dimentions initiales de la console:
    int HauteurInitiale = Console.WindowHeight;
    int LargeurInitiale = Console.WindowWidth;


    // On itère tant que l'utilisateur n'a pas touché une touche de clavier invalide.
    quitterPartie = false;
    while (!quitterPartie)
    {
        //Q#2
        // On verifie si les dimensions de la console ont changé
        if (Console.WindowHeight != HauteurInitiale || Console.WindowWidth != LargeurInitiale)
        {
            // Réinitialiser les dimensions sauvegardées
            // On sauve les nouvelles dimensions afin de reinitialiser la partie et continuer
            HauteurInitiale = Console.WindowHeight;
            LargeurInitiale = Console.WindowWidth;

            // Reinitialise la partie:
            initialiserPartie();
        }

        // On affiche le repas.
        afficherRepas();

        // Puis on affiche la souris.
        afficherSouris();

        // Gerer la collision
        collisionEntreSprites();

        // Consommer le repas
        consommerRepas();

        // On déplace la souris.
        quitterPartie = !gererInput();
    }

    // On s'apprête à quitter la partie ; on restaure donc la console à ses couleurs originales.
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.White;
}

/******************************************************************************************
 Description: Fonction qui lit les touches directionnelles pressée du clavier et déplace 
              la souris selon celles-ci. La valeur de retour indique si une touche valide
              fut pressée.
 Paramètres : Aucun.
 Retour     : Vrai si une touche de déplacement fut pressée ou tout autre a part "escape" ; faux sinon.
 ******************************************************************************************/
bool gererInput()
{
    // Gérer le jeu selon la touche de clavier pressée.
    switch (Console.ReadKey(true).Key)
    {
        case ConsoleKey.UpArrow:
            deplacerSouris(souris.x, souris.y - 1);
            return true;   // une touche valide fut pressée

        case ConsoleKey.DownArrow:
            deplacerSouris(souris.x, souris.y + 1);
            return true;   // une touche valide fut pressée

        case ConsoleKey.LeftArrow:
            deplacerSouris(souris.x - 1, souris.y);
            return true;   // une touche valide fut pressée

        case ConsoleKey.RightArrow:
            deplacerSouris(souris.x + 1, souris.y);
            return true;   // une touche valide fut pressée

        case ConsoleKey.Escape:
            return false; // La touche escape pour quiter le jeu

        default:   
            return true;   // n'importe quelle autre touche a part les touches directionnelles, ne font aucun effet.   
    }
}

/******************************************************************************************
 Description: Fonction qui déplace la souris à la position donnée en paramètre. La fonction
              efface la souris à sa position actuelle, mais ne l'affiche pas à sa nouvelle
              position.
 Paramètres : nouveauX - nouvelle position horizontale de la souris.
              nouveauY - nouvelle position verticale de la souris.
 Retour     : Aucun.
 ******************************************************************************************/
void deplacerSouris(int nouveauX, int nouveauY)
{
    // Efface les caractères affichés dans la console à la position actuelle de la souris.
    Console.SetCursorPosition(souris.x, souris.y);
    for (int i = 0; i < typesApparance[souris.index].Length; i++)
        Console.Write(" ");

    // Déterminer la distance de déplacement horizontal en fonction du type de nourriture consommée
    int deplacementHorizontal;
    switch (souris.index)
    {
        case 1: // Sur-vitaminée
            deplacementHorizontal = 3;
            break;
        case 2: // Malade
            deplacementHorizontal = 1;
            break;
        default: // État normal
            deplacementHorizontal = 2;
            break;
    }

    // Initialiser la nouvelle position horizontale à la position actuelle de la souris
    int nouvellePositionX = souris.x;

    // Vérifier si la nouvelle position souhaitée est à droite de la position actuelle
    if (nouveauX > souris.x)
    {
        // Si oui, déplacer la souris vers la droite en ajoutant la distance de déplacement
        nouvellePositionX = souris.x + deplacementHorizontal;
    }
    // Vérifier si la nouvelle position souhaitée est à gauche de la position actuelle
    else if (nouveauX < souris.x)
    {
        // Si oui, déplacer la souris vers la gauche en soustrayant la distance de déplacement
        nouvellePositionX = souris.x - deplacementHorizontal;
    }

    // Restreindre la nouvelle position de la souris aux limites de la console.
    souris.x = (nouvellePositionX < 0) ? 0 : (nouvellePositionX >= largeurConsole ? largeurConsole : nouvellePositionX);
    souris.y = (nouveauY < 0) ? 0 : (nouveauY >= hauteurConsole ? hauteurConsole : nouveauY);

    // Réafficher la souris à sa nouvelle position
    afficherSouris();
}

/******************************************************************************************
 Description: Affiche la souris selon sa position dans la console et son état.
 Paramètres : Aucun.
 Retour     : Aucun.
 ******************************************************************************************/
void afficherSouris()
{
    // Préparer la console pour l'affichage de la souris.
    Console.SetCursorPosition(souris.x, souris.y);
    Console.ForegroundColor = couleurs[souris.index];

    // Afficher la souris à sa nouvelle position.
    Console.Write(typesApparance[souris.index]);
}

/******************************************************************************************
 Description: Génère le repas pour la console.
 Paramètres : Aucun.
 Retour     : Aucun.
 ******************************************************************************************/
void produireRepas()
{
    repas.index = rnd.Next(0, typesNourriture.Length);

    //#Q3
    // Choisir aléatoirement une position horizontale dans la ligne courante.
    repas.x = rnd.Next(0, largeurConsole - typesNourriture[repas.index].Length);

    // Choisir aleatoirement une position verticale dans la ligne cournate en ignorant la prmmiere ligne:
    repas.y = rnd.Next(1, hauteurConsole);
}

/******************************************************************************************
 Description: Affiche le repas selon sa position dans la console et son type de nourriture.
 Paramètres : Aucun.
 Retour     : Aucun.
 ******************************************************************************************/
void afficherRepas()
{
    Console.SetCursorPosition(repas.x, repas.y);
    Console.ForegroundColor = couleurs[repas.index];
    Console.Write(typesNourriture[repas.index]);
}

/******************************************************************************************
 Description: Fonction qui prépare la console, produit le repas et positionne la souris.
 Paramètres : Aucun.
 Retour     : Aucun.
 ******************************************************************************************/
void initialiserPartie()
{
    // Sauvegarder les dimensions de la console afin de détecter les redimensionnements.
    hauteurConsole = Console.WindowHeight - 1;
    largeurConsole = Console.WindowWidth - 5;

    // On efface le contenu de la console.
    Console.Clear();

    // Générer et afficher un repas.
    produireRepas();

    // Positionner la souris au coin supérieur gauche de la console.
    deplacerSouris(0, 0);
}

/******************************************************************************************
 Description: Fonction qui dectecte la collision entre les sprites
 Paramètres : Aucun.
 Retour     : vrai s'il ya collision; faux sinon.
 ******************************************************************************************/

bool collisionEntreSprites()
{
    if (souris.y == repas.y) // verifie si les sprites sont sur la même ligne
    {
        if (souris.x < repas.x && (repas.x - souris.x) < typesApparance[souris.index].Length)   // si la souris est avant et que leur distance est moindre que la longeur de la
                                                                                                // souris:
        {
            return true; // Il y'collision
        }
        else if (souris.x >= repas.x && (souris.x - repas.x) < typesNourriture[repas.index].Length) // si la souris est apres et que leur distance est moindre que la longeur du
                                                                                                    // du repas:
        {
            return true; // Il y'a collision
        }
    }
    return false;
}

/******************************************************************************************
 Description: Fonction qui veerifie si la collision est effectif, puis la souris consomme le repas
               le repas consommé disparait puis se repositionne dans un autre endroit
 Paramètres : Aucun.
 Retour     : Aucun.
 ******************************************************************************************/
void consommerRepas()
{
    if (collisionEntreSprites())
    {
        // Effacer le repas consommé
        Console.SetCursorPosition(repas.x, repas.y);
        Console.Write(new string(' ', typesNourriture[repas.index].Length));

        // Changer l'apparence de la souris
        souris.index = repas.index;

        // Produire un nouveau repas
        produireRepas();

        // Afficher le nouveau repas immédiatement
        afficherRepas();

        // Réafficher la souris avec sa nouvelle apparence
        afficherSouris();
    }
}

//voici mon code