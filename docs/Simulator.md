# 🧪 **Simulator**

---

## ⚙️ **Simulation Mode 설정 방법**

<p align="center">
  <img width="1160" height="746" alt="image" src="https://github.com/user-attachments/assets/8f354bee-afd9-4be6-9aa3-3a9d507e4650" />
</p>

> **경로:** `Menu → Simulation Setting`  
> **설정:** `Simulation Mode = ON` → 시뮬레이션 기능 활성화

<p align="center">
  <img width="350" alt="simulation-mode-on" src="https://github.com/user-attachments/assets/65a9112e-d247-42c6-9588-5ffcbba85399" />
</p>

---

## 🔌 **VirtualPLC 연결 방법**

> ⚠️ **진행 전:** [VirtualPLC](https://github.com/NHSE/VirtualPLC/tree/master)가 **실행 중**이어야 합니다.

1. Simulation Window 내 **PLC 클릭 → Connect 클릭**  
2. VirtualPLC와 SemiConductor-Equipment가 아래와 같은 상태라면 연결 완료 ✅

<p align="center">
  <img width="500" alt="plc-connected" src="https://github.com/user-attachments/assets/9b5ed668-de5a-4b9c-b3e4-2be685b3f138" />
</p>

---

## 🚚 **VirtualOHT 연결 방법**

> ⚠️ **진행 전:** [VirtualOHT](https://github.com/NHSE/VirtualOHT/tree/master)가 **실행 중**이어야 합니다.

1. Simulation Window 내 **OHT 클릭 → Connect 클릭**  
2. VirtualOHT 프로그램 내 **Connect 클릭**

> VirtualOHT와 SemiConductor-Equipment가 아래와 같은 상태라면 연결 완료 ✅

<p align="center">
  <img width="700" alt="oht-connected" src="https://github.com/user-attachments/assets/79d758d1-9f3f-4764-8362-749ec7422841" />
</p>

---

## 🧰 **VirtualOHT 사용 방법**

> ⚠️ VirtualOHT와 SemiConductor-Equipment가 **서로 연결된 상태**여야 합니다.

---

### 🪜 **LoadPort 설정**

하기 이미지와 같이 탭을 눌러 **Carrier Load / UnLoad** 대상 **LoadPort**를 설정할 수 있습니다.

<p align="center">
  <img width="1355" height="898" alt="loadport-setting" src="https://github.com/user-attachments/assets/efb1936e-8ee3-4dc9-8bb6-4f700a3c7833" />
</p>

---

### 💽 **Carrier 내 Wafer 설정**  
([Wafer 설정 가이드](docs/LoadPort.md)와 동일)

1. 아래 이미지와 같이 **Setup 버튼 클릭**
   
   <p align="center">
     <img width="600" alt="wafer-setup" src="https://github.com/user-attachments/assets/118d58be-0860-4808-818d-db0161fd4e95" />
   </p>

2. **Wafer Loader Window**에서 Wafer 설정
   
   <p align="center">
     <img width="300" alt="wafer-loader" src="https://github.com/user-attachments/assets/b8a99aeb-25e6-4292-8574-e0b3511fd057" />
   </p>

---

### 📦 **LP 내 Carrier Load / UnLoad**

| 버튼 | 동작 설명 |
|:-----:|:-----------|
| **Load** | 설정된 LP에 **Wafer가 담긴 Carrier 전송**, SemiConductor-Equipment 프로그램 내 LP에 **Wafer 정보 등록** |
| **UnLoad** | 설정된 LP의 **Carrier 회수**, SemiConductor-Equipment 프로그램 내 LP의 **Wafer 정보 삭제** |

---
