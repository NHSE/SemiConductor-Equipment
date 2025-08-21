pipeline {
    agent any

    environment {
        CONFIG = 'Release'
        SOLUTION_DIR = 'SemiConductor Equipment'       // .sln 파일 있는 폴더
        SOLUTION_FILE = 'SemiConductor Equipment.sln' // 공백 포함 파일명
    }

    stages {
        stage('Checkout') {
            steps {
                git credentialsId: 'SemiConductor-Equipment',
                    branch: 'master',
                    url: 'https://github.com/NHSE/SemiConductor-Equipment.git'
            }
        }

        stage('Restore') {
            steps {
                echo 'Restoring NuGet packages...'
                dir("${SOLUTION_DIR}") {
                    bat "\"C:\\Program Files\\dotnet\\dotnet.exe\" restore \"${SOLUTION_FILE}\""
                }
            }
        }

        stage('Test') {
            steps {
                echo 'Running unit tests...'
                dir("${SOLUTION_DIR}") {
                    bat "\"C:\\Program Files\\dotnet\\dotnet.exe\" test \"${SOLUTION_FILE}\" --configuration ${CONFIG}\""
                }
            }
        }

        stage('Build') {
            steps {
                echo 'Building project...'
                dir("${SOLUTION_DIR}") {
                    bat "\"C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\MSBuild\\Current\\Bin\\MSBuild.exe\" \"${SOLUTION_FILE}\" /p:Configuration=${CONFIG}"
                }
            }
        }

        stage('Docker Build') {
            steps {
                echo 'Docker build stage (skipped if Docker not installed)'
            }
        }
    }

    post {
        always {
            echo 'Pipeline finished.'
        }
        success {
            echo 'Pipeline succeeded!'
        }
        failure {
            echo 'Pipeline failed!'
        }
    }
}
