/* groovylint-disable-next-line CompileStatic */
pipeline {
    agent
    {
        label 'Linux'
    }

    stages {
        stage('Checkout') {
            steps {
                // Étape pour récupérer le code depuis le référentiel Git
                // A voir !!
                checkout scm
            }
        }

        stage('Build') {
            steps {
                // Étape pour compiler le code (remplacez cette section par votre propre logique de build)
                echo 'Building...'
                sh 'ls /var/lib/jenkins/'
                script: {
                sh 'dotnet clean'
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
                    /* groovylint-disable-next-line LineLength */
                    // Étape pour déployer l'application (remplacez cette section par votre propre logique de déploiement)
                    echo "C'est l'étape de déploiement ici"
                }
            }
        }

        post {
            // Actions à effectuer après l'exécution du pipeline
            success {
                echo 'Le pipeline s\'est exécuté avec succès!'
                sh 'rm -f Jenkinsfile'

            // Ajoutez ici des actions supplémentaires à effectuer en cas de succès
            }

            failure {
                echo 'Le pipeline a échoué!'

            // Ajoutez ici des actions supplémentaires à effectuer en cas d'échec
            }
        }
    }
}

