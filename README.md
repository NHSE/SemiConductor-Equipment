# SemiConductor-Equipment

반도체 장비 제어 및 모니터링을 위한 C# WPF 기반 프로그램입니다.  
SECS/GEM 통신을 포함하여 장비 상태 모니터링, 데이터 관리, 공정 흐름 시뮬레이션 기능을 제공합니다.


---

## 📑 목차
- [프로그램 설명](#-프로그램-설명)
- [목적](#-목적)
- [사용 방법](#-사용-방법)
- [시뮬레이터 결과](#-시뮬레이터-결과-파일)
- [프로젝트 구조](#-프로젝트-구조)
  
![SemiConductor-Equipment](https://github.com/user-attachments/assets/436c2bb8-e451-40b8-9ea6-a51d101abc57)


---




## 📌 프로그램 설명
이 프로그램은 반도체 장비 소프트웨어 개발을 위한 **시뮬레이터 및 제어 애플리케이션**입니다.  
주요 기능은 다음과 같습니다:

- **WPF 기반 사용자 인터페이스**  
- **Robot Arm, Load Port, Clean Chamber, Dry Chamber 동작 시뮬레이션**  
- **데이터 로깅 및 CSV 저장**  
- **SECS/GEM 표준 통신 지원 (Secs4Net 기반)**  
- **CI/CD 환경(Jenkins) 연동**  
- **추후 Modbus 통신 추가 예정**

---

## 🎯 목적
- **윈도우 애플리케이션 개발 역량 습득**
- **반도체 장비 동작 원리 학습 및 시뮬레이션**  
- **SECS/GEM 표준 통신 프로토콜 구현**  
- **Jenkins를 활용한 자동화 빌드/배포 파이프라인 구축**  

---

## 🛠 사용 방법
### 1. 저장소 Clone 및 설치 방법
```bash
(git, dotnet이 설치가 되어 있지 않다면 설치 후 진행해주세요)

git clone --branch develop https://github.com/NHSE/SemiConductor-Equipment.git
cd SemiConductor-Equipment/SemiConductor Equipment

2. 빌드
dotnet build

3. 실행
dotnet run
```

### 2. 📡 SECS/GEM 통신 방법

Secs4Net 라이브러리를 이용한 SECS/GEM 구현

SwingSecsSimulator와 연동 가능 (https://kenta-shimizu.github.io/secs-simulator/index.html)

➡️ [SECS/GEM 통신 상세 문서](docs/SECSGEM.md)

### 3. 🔌 시뮬레이터 사용 방법

TCP/IP 기반 통신 지원

비동기 Task 기반 내부 이벤트 처리

CSV 데이터 입출력 지원

➡️ [사용 방법 상세 문서](docs/GETTING_STARTED.md)

## 🗂 시뮬레이터 결과 파일

### 기본 로그 (C:/Logs/)
| 파일/폴더 | 설명 | 파일명 |
|-----------|------|--------|
| **SECS/GEM Log** | SECS/GEM 통신 메시지 기록 | SECS_yyyyMMdd.log |
| **Trace Data Log** | 장비 동작 추적 데이터 기록 | Trace_yyyyMMdd.log |
| **Event Data Log** | 장비 이벤트 발생 기록 | Event_yyyyMMdd.log |
| **Robot Data Log** | Robot Arm 작동 기록 | RobotArm_yyyyMMdd.log |

### 세부 로그 (C:/Logs/yyMMdd-hhmmss/)  
*폴더 명은 S14F9 메시지 수신 시점을 기준으로 자동 생성됩니다.*

| 파일/폴더 | 설명 | 파일명 |
|-----------|------|--------|
| **Clean/Dry Chamber Log** | 각 챔버 클린/드라이 공정 기록 |{Chamber Type}yyyyMMdd_hhmmss.log|
| **Wafer Result Log** | 웨이퍼 처리 결과 기록 |yyyyMMdd_hhmmss_{Chamber Type}_Result.csv|

## 🗂 프로젝트 구조

프로젝트 구조는 UML 다이어그램으로 표현합니다.

## 📬 개발자

Author: NHSE (남현석)

Email: dlfnqwe159@gmail.com
