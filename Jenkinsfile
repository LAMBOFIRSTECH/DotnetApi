/* groovylint-disable-next-line CompileStatic */
pipeline {
    agent { label 'Linux' } 
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
                    rm *.txt *.png *.md
                    mkdir -p ${COVERAGE_PATH}
                    chmod -R 777 ${COVERAGE_PATH}
    
                '''
            }
        }
        stage('Build de l\'image docker') {
            steps {
                sh '''
                   docker build -t api-tasks .
                   '''
            }
        }
        stage('Exécution des tests') {
            steps {
                // Exécuter les tests dans une nouvelle image construite à partir de l'image de construction
                /* groovylint-disable-next-line GStringExpressionWithinString */
                /* groovylint-disable-next-line LineLength */
                script {
                    /* groovylint-disable-next-line NestedBlockDepth */
                    try {
                        /* groovylint-disable-next-line GStringExpressionWithinString, LineLength */
                        sh 'docker run --rm -v ${WORKSPACE_DIR}/TestResults:/TestResults api-tasks dotnet test TasksManagement_Tests/TasksManagement_Tests.csproj --no-build --collect:"XPlat Code Coverage" --results-directory /TestResults -v d'
                    /* groovylint-disable-next-line CatchException, NestedBlockDepth */
                    } catch (Exception e) {
                        currentBuild.result = 'FAILURE'
                        throw e
                    }
                }
            }
        }
        stage('Vérification via SonarQube ') {
            steps {
                script {
                    /* groovylint-disable-next-line GStringExpressionWithinString */
                    sh '''
                        if ! find ${COVERAGE_PATH} -type f -name 'coverage.cobertura.xml'; then
                            echo "Le rapport analytique introuvable*****************"
                            exit 1
                        fi
                    '''
                    /* groovylint-disable-next-line NestedBlockDepth */
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
        stage('Démarrage du conteneur docker') {
            steps {
                /* groovylint-disable-next-line GStringExpressionWithinString */
                sh '''
                   docker run -d -p 5195:5195 -p 7251:7251 --name ${PROJECT_NAME} api-tasks
                   '''
            }
        }
    }

    post {
        success {
            echo 'Le pipeline s\'est exécuté avec succès!'
            sh 'rm -f Jenkinsfile' // Enlevez le Jenkinsfile si nécessaire
            sh 'rm -f Docker*' // Enlevez le Dockerfile si nécessaire
        }
        failure {
            echo 'Le pipeline a échoué!'
        }
    }
}
