/* groovylint-disable NglParseError */
/* groovylint-disable-next-line CompileStatic */
pipeline {
    agent { label 'Linux' }
    environment {
        WORKSPACE_DIR = "${env.WORKSPACE}/Dotnet-Api-TasksManagement" // Définir la variable d'environnement
        API_DIR = "${WORKSPACE_DIR}/TasksManagement_API"
        APP_NAME = 'TasksManagement_API'
        scannerHome = tool 'sonarscanner'
    }

    stages {
        stage('Clonage du référentiel GitHub') {
            steps {
                // Étape pour récupérer le code depuis le référentiel Git
                checkout scm
            }
        }
        stage('Pré-traitement ') {
            steps {
                /* groovylint-disable-next-line GStringExpressionWithinString */
                sh '''
                    mkdir -p Certs
                    cp ../Certs/ApiNet6Certificate.pfx ./Certs/
                   '''
            }
        }
        stage('Vérification via SonarQube ') {
            steps {
                script {
                    /* groovylint-disable-next-line NestedBlockDepth */
                    withSonarQubeEnv('SonarQube-Server') {
                        sh """
                            ${scannerHome}/sonar-scanner-5.0.1.3006/bin/sonar-scanner \
                            -Dsonar.projectKey=sonar ${APP_NAME}-project-1 \
                            -Dsonar.projectName="${APP_NAME}" \
                            -Dsonar.projectVersion=1.0 \
                            -Dsonar.sources=${API_DIR} \
                            -Dsonar.language=cs \
                            -Dsonar.sourceEncoding=UTF-8 \
                            -Dsonar.cs.opencover.reportsPaths=${API_DIR}/**/coverage.opencover.xml \
                            -Dsonar.cs.vstest.reportsPaths=${API_DIR}/**/*.trx
                        """
                    }
                }
            }
        }

        stage("Build de l'image docker") {
            steps {
                /* groovylint-disable-next-line GStringExpressionWithinString */
                //Pousser l'image sur une registry

                sh '''
                   docker build -t api-tasks .
                   '''
            }
        }
        stage('Démarrage du conteneur docker') {
            steps {
                /* groovylint-disable-next-line GStringExpressionWithinString */
                sh '''
                   docker run -d -p 5163:5163 -p 7082:7082 --name ${APP_NAME} api-tasks
                   '''
            }
        }

        stage('Lancement de Test Unitaires') {
            steps {
                // Étape pour exécuter les tests (remplacez cette section par votre propre logique de test)
                sh '''
                   whoami
                   '''
            }
        }

        stage('Deploy') {
            steps {
                // Étape pour déployer l'application (remplacez cette section par votre propre logique de déploiement)
                echo "C'est l'étape de déploiement ici"
                /* groovylint-disable-next-line DuplicateStringLiteral, GStringExpressionWithinString */
                sh '''
                   ls -ld *
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
