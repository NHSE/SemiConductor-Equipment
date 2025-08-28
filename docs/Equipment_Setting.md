# Equipment Setting 화면 구성

<p align="center">
  <img width="1323" height="834" alt="image" src="https://github.com/user-attachments/assets/b1a31e1f-1ff0-47ba-bdcc-4c44dbabec8b" />
</p>

## 🛠️ Chamber Process Parameters

- **⚙️ RPM**  
  Wafer가 Chamber에 삽입될 때 회전하는 속도 (RPM).  

- **💧 Chemical Flow Rate / Pre-Clean Flow Rate**  
  Clean Chamber에서 사용되는 Chemical / Pre-Clean의 초당 유량.  
  - 유량이 0이 되면 해당 Wafer는 **Error State** → Chamber는 **Disable** 상태 전환  
  - 모든 Clean Chamber의 Chemical 또는 Pre-Clean의 유량이 0이 되면 전체 Process **종료**  

- **🌊 Chemical Spray Time / Pre-Clean Spray Time**  
  Clean Chamber에서 Chemical / Pre-Clean이 분사되는 시간.  

- **🔥 Min Temperature / Max Temperature**  
  Dry Chamber에서 Process 성공 여부를 판정하는 최소·최대 온도.  

- **⏱️ Chamber Time**  
  Dry Chamber에서 Process가 진행되는 시간.  
