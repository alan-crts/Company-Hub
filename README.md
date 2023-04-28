# Company-Hub

Ce projet est un annuaire des employés d'une entreprise réalisé avec .NET Razor Pages MVC, une base de données PostgreSQL et un reverse proxy Nginx. Il utilise également Docker pour la gestion de l'environnement.

## Prérequis

- Docker

## Lancement du projet

Le projet peut être lancé avec les commandes suivantes :

```bash
make start  # Lance les conteneurs Docker
make stop   # Arrête les conteneurs Docker
make bash   # Permet d'accéder au bash du backend
make install # Installe les dépendances du backend
```

## Fonctionnalités

Le projet comprend les fonctionnalités suivantes :

- Affichage de la liste des employés de l'entreprise avec leurs informations et CRUD
- Affichage de la liste des services et CRUD
- Affichage de la liste des sites et CRUD
- Recherche d'employés par nom, numéro de téléphone mobile/fix et adresse mail

## Technologies utilisées

- .NET Razor Pages MVC
- PostgreSQL pour la base de données
- Docker pour la gestion de l'environnement
- Nginx pour le reverse proxy
