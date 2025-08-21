pipeline {
    agent any
    stages {
        stage('Prepare'){
            steps {
                git credentialsId : 'SemiConductor-Equipment',
                    branch : 'master',
                    url : 'https://github.com/NHSE/SemiConductor-Equipment.git'
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