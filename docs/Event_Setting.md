# Event Setting 화면 구성

<p align="center">
  <img width="1406" height="904" alt="image" src="https://github.com/user-attachments/assets/1f3cead1-1c4e-48d2-921a-edabbb260e7e" />
</p>

## 🔧 기능 설명
- CEID와 RPTID를 **수정**하거나, RPTID 및 VID를 **추가**할 수 있는 화면입니다.
- 각 항목의 의미:
  - **CEID (Collection Event ID)** : 특정 이벤트 발생을 나타내는 고유 ID  
  - **RPTID (Report ID)** : 특정 이벤트에 묶여 전달되는 Report의 ID  
  - **VID (Variable ID)** : Report에 포함될 수 있는 데이터의 ID  

## 🖱️ 화면 조작
- **빨간 박스 (Name 탭)** → [CEID 수정](#-ceid-수정)  
- **노란 박스 (VID 탭)** → [RPTID 수정](#-rptid-수정)  
- **초록 박스** → [RPTID 추가](#-rptid-추가)  
- **사용 가능한 CEID 목록** → [CEID LIST](#-사용-가능한-ceid-list)  
- **사용 가능한 VID 목록** → [VID LIST](#-사용-가능한-vid-list)  

---

## ✏️ CEID 수정
<p align="center">
  <img width="408" height="337" alt="image" src="https://github.com/user-attachments/assets/aa39db3e-8dd8-4c03-bfeb-eb69a43d4376" />
</p>

---

## ✏️ RPTID 수정
<p align="center">
  <img width="408" height="337" alt="image" src="https://github.com/user-attachments/assets/c7314700-189d-4a90-88ec-df9f48091bc6" />
</p>

---

## ➕ RPTID 추가
<p align="center">
  <img width="438" height="367" alt="image" src="https://github.com/user-attachments/assets/1acd29af-c2df-4f18-aa90-12adf05646d7" />
</p>

---

## 📋 사용 가능한 CEID LIST
| CEID Number | Name | 발생 시점 |
|-------------|------|------------|
| **100** | Load Port Load Complete | Load Port(Carrier)에 Wafer 삽입 시 |
| **101** | Load Port Unload Complete | Load Port(Carrier)에서 Wafer 추출 시 |
| **200** | Wafer Move Start | Wafer가 RobotArm에 의해 이동 시작 |
| **201** | Wafer Move Complete | Wafer가 RobotArm에 의해 이동 완료 |
| **300** | Process Start | Chamber에서 공정 시작 |
| **301** | Process Complete | Chamber에서 공정 종료 |

---

## 📋 사용 가능한 VID LIST
| VID Number | Name | 종류 | 데이터 저장 시점 | 비고 |
|-------------|------|------|----------------|------|
| **100** | EquipmentStatus | SVID | 프로그램 시작 | |
| **101** | RobotStatus | SVID | 프로그램 시작 | |
| **102** | Loadport1_DoorStatus | SVID | 프로그램 시작 | |
| **103** | Loadport2_DoorStatus | SVID | 프로그램 시작 | |
| **104** | Chamber1_Chemical | SVID | 프로그램 시작 | |
| **105** | Chamber2_Chemical | SVID | 프로그램 시작 | |
| **106** | Chamber3_Chemical | SVID | 프로그램 시작 | |
| **107** | Chamber4_Chemical | SVID | 프로그램 시작 | |
| **108** | Chamber5_Chemical | SVID | 프로그램 시작 | |
| **109** | Chamber6_Chemical | SVID | 프로그램 시작 | |
| **110** | Chamber1_Pre_Clean | SVID | 프로그램 시작 | |
| **111** | Chamber2_Pre_Clean | SVID | 프로그램 시작 | |
| **112** | Chamber3_Pre_Clean | SVID | 프로그램 시작 | |
| **113** | Chamber4_Pre_Clean | SVID | 프로그램 시작 | |
| **114** | Chamber5_Pre_Clean | SVID | 프로그램 시작 | |
| **115** | Chamber6_Pre_Clean | SVID | 프로그램 시작 | |
| **116** | SWVersion | SVID | 프로그램 시작 | |
| **1001** | WaferTemp | DVID | Carrier에 Wafer 삽입 이후 | |
| **1002** | LoadportWaferCount | DVID | Carrier에 Wafer 삽입 이후 | |
| **1003** | RecipeData | DVID |  | 추후 Recipe 기능 개발 예정 |
| **1005** | WaferId | DVID | PJ ID 생성 이후 (S16F11) | |
| **1007** | WaferPosition | DVID | Carrier에 Wafer 삽입 이후 | |
| **1008** | PJID | DVID | PJ ID 생성 이후 (S16F11) | |
| **1009** | CJID | DVID | CJ ID 생성 이후 (S14F9) | |
| **1010** | CarrierID | DVID | Carrier ID 생성 (S3F17) | |
