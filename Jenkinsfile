pipeline {
    agent any

    environment {
        CONFIG = 'Release'
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
                dir('SemiConductor Equipment') {
                    bat '"C:\\Program Files\\dotnet\\dotnet.exe" restore "SemiConductor-Equipment.sln"'
                }
            }
        }

        stage('Test') {
            steps {
                echo 'Running unit tests...'
                dir('SemiConductor Equipment') {
                    bat '"C:\\Program Files\\dotnet\\dotnet.exe" test "SemiConductor-Equipment.sln" --configuration %CONFIG%'
                }
            }
        }

        stage('Build') {
            steps {
                echo 'Building project...'
                dir('SemiConductor Equipment') {
                    bat '"C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\MSBuild\\Current\\Bin\\MSBuild.exe" "SemiConductor-Equipment.sln" /p:Configuration=%CONFIG%'
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
