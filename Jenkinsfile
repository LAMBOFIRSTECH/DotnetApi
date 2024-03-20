pipeline {
    agent any

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
                sh 'ls /var/lib/jenkins/'
                script: {
                  /* groovylint-disable-next-line LineLength */
                sh 'docker run -d -p 5163:5163 -p 7082:7082 -e ASPNETCORE_HTTP_PORT=5163 -e ASPNETCORE_URLS=http://+:5163 --name API arturlambodocker/tasksmanagement_api:v1.0'
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

