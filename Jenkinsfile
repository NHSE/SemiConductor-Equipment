pipeline {
    agent any

    environment {
        SOLUTION = '"SemiConductor Equipment\\SemiConductor-Equipment.sln"'
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
        dir('SemiConductor Equipment') {   // .sln이 있는 폴더로 이동
            bat "\"C:\\Program Files\\dotnet\\dotnet.exe\" test \"SemiConductor Equipment.sln\" --configuration %CONFIG%"
        }
    }
}

stage('Build') {
    steps {
        echo 'Building project...'
        dir('SemiConductor Equipment') {
            bat "\"C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\MSBuild\\Current\\Bin\\MSBuild.exe\" \"SemiConductor Equipment.sln\" /p:Configuration=%CONFIG%"
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