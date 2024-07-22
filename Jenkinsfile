pipeline {
    agent { label 'Linux' } // Assurez-vous que Docker est installé sur cet agent
    environment {
        WORKSPACE_DIR = "${env.WORKSPACE}"
        API_DIR = "${WORKSPACE_DIR}/TasksManagement_API"
        scannerHome = tool 'sonarscanner'
        PROJECT_KEY = 'Sonar-web-api'
        PROJECT_NAME = 'WEB_API'
        PROJECT_VERSION = '1.0'
        SONAR_SCANNER_PATH = "${scannerHome}/sonar-scanner-5.0.1.3006/bin/sonar-scanner"
        SONAR_LANGUAGE = 'cs'
        SONAR_ENCODING = 'UTF-8'
        COVERAGE_PATH = "${WORKSPACE_DIR}/TestResults"
        OPENCOVER_REPORT_PATH = "${COVERAGE_PATH}/**/coverage.cobertura.xml"
        VSTEST_REPORT_PATH = "${COVERAGE_PATH}/**/*.trx"
    }

    stages {
        stage('Clonage du référentiel GitHub') {
            steps {
                checkout scm
            }
        }
        stage('Pré-traitement') {
            steps {
                sh '''
                    mkdir -p Certs
                    cp ../Certs/ApiNet6Certificate.pfx ./Certs/
                '''
            }
        }
        stage('Build de l\'image docker') {
            steps {
                sh '''
                   docker build -t api-tasks -f Dockerfile .
                   '''
            }
        }
        stage('Démarrage du conteneur docker') {
            steps {
                sh '''
                   docker run -d -p 5163:5163 -p 7082:7082 --name ${PROJECT_NAME} api-tasks
                   '''
            }
        }
        stage('Tests et analyse de la couverture de code') {
            steps {
                script {
                    sh '''
                        docker exec ${PROJECT_NAME} dotnet test --no-build --collect:"XPlat Code Coverage" --results-directory /TestResults
                    '''
                }
            }
        }
        stage('Vérification via SonarQube ') {
            steps {
                script {
                    sh '''
                        if ! find ${COVERAGE_PATH} -type f -name 'coverage.cobertura.xml'; then
                            echo "Le rapport analytique introuvable*****************"
                            exit 1
                        fi
                    '''
                    withSonarQubeEnv('SonarQube-Server') {
                        sh """
                            ${SONAR_SCANNER_PATH} \
                            -Dsonar.projectKey=${PROJECT_KEY} \
                            -Dsonar.projectName="${PROJECT_NAME}" \
                            -Dsonar.projectVersion=${PROJECT_VERSION} \
                            -Dsonar.sources=${API_DIR} \
                            -Dsonar.language=${SONAR_LANGUAGE} \
                            -Dsonar.sourceEncoding=${SONAR_ENCODING} \
                            -Dsonar.cs.opencover.reportsPaths=${OPENCOVER_REPORT_PATH} \
                            -Dsonar.cs.vstest.reportsPaths=${VSTEST_REPORT_PATH}
                        """
                    }
                }
            }
        }
    }

    post {
        success {
            echo 'Le pipeline s\'est exécuté avec succès!'
            sh 'rm -f Jenkinsfile' // Enlevez le Jenkinsfile si nécessaire
            sh 'rm -f Dockerfile' // Enlevez le Dockerfile si nécessaire
            sh 'rm  *.sh *.txt *.png *.md' // Enlevez les fichiers temporaires si nécessaire
        }
        failure {
            echo 'Le pipeline a échoué!'
        }
    }
}
