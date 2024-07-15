/* groovylint-disable-next-line CompileStatic */
pipeline {
    agent { label 'Linux' }
    environment {
        WORKSPACE_DIR = "${env.WORKSPACE}/Dotnet-Api-TasksManagement" // Définir la variable d'environnement
        API_DIR = "${WORKSPACE_DIR}/TasksManagement_API"
        APP_NAME = "TasksManagement_API"
    }

    stages {
        stage('Checkout') {
            steps {
                // Étape pour récupérer le code depuis le référentiel Git
                checkout scm
            }
        }

        stage('Build ') {
            steps {
                /* groovylint-disable-next-line GStringExpressionWithinString */
                sh '''
                   cd ${WORKSPACE_DIR}
                   ls
                   '''
            }
        }

        stage('Lancement de Test Unitaires') {
            steps {
                // Étape pour exécuter les tests (remplacez cette section par votre propre logique de test)
                sh '''
                   ls -ld
                   '''
            }
        }

        stage('Deploy') {
            steps {
                // Étape pour déployer l'application (remplacez cette section par votre propre logique de déploiement)
                echo "C'est l'étape de déploiement ici"
                /* groovylint-disable-next-line DuplicateStringLiteral, GStringExpressionWithinString */
                sh '''
                   ls -ld
                   '''
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
        }
    }
}
