pipeline {
    agent any

    environment {
        SOLUTION = 'SemiConductor-Equipment.sln'
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

        stage('Test') {
            steps {
                echo 'Running unit tests...'
                // 실제 테스트 실행
                bat "\"C:\\Program Files\\dotnet\\dotnet.exe\" test %SOLUTION% --configuration %CONFIG%"
            }
        }

        stage('Build') {
            steps {
                echo 'Building project...'
                // 실제 빌드 실행
                bat "\"C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\MSBuild\\Current\\Bin\\MSBuild.exe\" %SOLUTION% /p:Configuration=%CONFIG%"
            }
        }

        stage('Docker Build') {
            steps {
                echo 'Docker build stage (skipped if Docker not installed)'
                // 실제 Docker 환경이면 아래 주석 해제
                // bat 'docker build -t my-image:latest .'
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