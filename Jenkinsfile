pipeline {
    agent any
    stages {
        stage('Prepare'){
            steps {
                git credentialsId : '{credentialId}',
                    branch : '{branchName}',
                    url : 'https://github.com/{repoName}/{projectName}.git'
            }
        }
        stage('test') {
            steps {
                echo 'test stage'
            }
        }
        stage('build') {
            steps {
                echo 'build stage'
            }
        }
        stage('docker build') {
            steps {
                echo 'docker build stage'
            }
        }
    }
}