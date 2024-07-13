/* groovylint-disable-next-line CompileStatic */
pipeline {
    agent {
        label 'Any'
    }

    stages {
        stage('Checkout') {
            steps {
                // Étape pour récupérer le code depuis le référentiel Git
                checkout scm
            }
        }

        stage('Build') {
            steps {
                // Étape pour compiler le code (remplacez cette section par votre propre logique de build)
                echo 'Building...'
                sh 'ls /var/lib/jenkins/workspace/Dotnet-Api-TasksManagement/'
                
                //sh 'dotnet clean'  // Corrigé : Utilisez simplement sh 'dotnet clean' sans le bloc script
            }
        }

        stage('Test') {
            steps {
                // Étape pour exécuter les tests (remplacez cette section par votre propre logique de test)
                // Lancer le dotnet test sur l'application.
                echo "C'est l'étape de test ici"
            }
        }

        stage('Deploy') {
            steps {
                // Étape pour déployer l'application (remplacez cette section par votre propre logique de déploiement)
                echo "C'est l'étape de déploiement ici"
            }
        }
    }

    post {
        // Actions à effectuer après l'exécution du pipeline
        success {
            echo 'Le pipeline s\'est exécuté avec succès!'
            sh 'rm -f Jenkinsfile' // Enlevez le Jenkinsfile si nécessaire
            sh 'rm -f Dockerfile' // Enlevez le Dockerfile si nécessaire
            sh 'rm  *.sh *.txt *.png *.md' // Enlevez le Dockerfile si nécessaire
        }

        failure {
            echo 'Le pipeline a échoué!'
            // Ajoutez ici des actions supplémentaires à effectuer en cas d'échec
        }
    }
}
