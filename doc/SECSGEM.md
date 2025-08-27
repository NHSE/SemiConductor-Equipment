# SECS/GEM 통신 방법

---

## 1. SwingSecsSimulator 설정 방법
1. SwingSecsSimulator를 설치 후 실행합니다.
2. `Set Config` 클릭
3. 하기의 그림과 같이 설정 후 OK를 눌러줍니다.
   
   <img width="459" height="477" alt="image" src="https://github.com/user-attachments/assets/f2d59036-338a-45bc-945a-12d422e2d937" />


---

## 2. 통신 연결 방법
1. 반도체 장비 프로그램 실행
2. SwingSecsSimulator에서 `OPEN` 클릭
3. 통신 연결되지 않을 시:
   - 장비 프로그램에서 IP 확인 및 설정 (Menu - IP Setting)
   - 메인화면에서 `Disconnect` → `Connect` 순으로 클릭

---

## 3. 구현된 SECS/GEM 메시지 종류

### 3.1 S2F23
➡️ Trace Data 설정용 메시지  

```text
S2F23 W
<L [5]
    <A "1">
    <A "000001">
    <U4 10>
    <U4 1>
    <L
        <U4 104>
        <U4 105>
        <U4 106>
        <U4 107>
        <U4 108>
        <U4 109>
    >
>.
```

### 3.2 S2F33
➡️ RPTID 설정 메세지

```text
S2F33 W
<L [2]
  <U4AUTO [1] >
  <L [2]
    <L [2]
      <U4 [1] 1>
      <L [2]
        <U4 [1] 101>
        <U4 [1] 102>
      >
    >
    <L [2]
      <U4 [1] 2>
      <L [2]
        <U4 [1] 100>
        <U4 [1] 102>
      >
    >
  >
>.
```

## 3.3 S2F35
➡️ CEID - RPTID Link 메세지

```text
S2F23 W
<L [5]
    <A "1">
    <A "000001">
    <U4 10>
    <U4 1>
    <L
        <U4 104>
        <U4 105>
        <U4 106>
        <U4 107>
        <U4 108>
        <U4 109>
    >
>.
```

## 3.4 S2F37
➡️ CEID Enable/Disable 설정 메세지

```text
S2F37 W
<L [2]
  <BOOLEAN [1] FALSE>
  <L [0]>
>.
```
## 3.5 S3F17
➡️ Carrier ID 설정 메세지

```text
S3F17 W
<L [5]
  <U4 [1] 1>
  <A "ProceedWithCarrier">
  <A "CSX 52078">
  <U1 [1] 1>
  <L [1]
    <L [2]
      <A "Usage">
      <A "product">
    >
  >
>.
```
## 3.6 S14F9
➡️ Control Job ID 설정 메세지
(반도체 장비 프로그램은 AUTO Start로만 구현하였으며, Recipe는 추후 구현 예정)

```text
S14F9 W
<L [3]
  <A "">
  <A "ControlJob">
  <L [5]
    <L [2]
        <A "ObjID">
        <A "cj00001">
    >
    <L [2]
        <A "CarrierInputSpec">
        <L [1]
            <A "CSX 52078">
        >
    >
    <L [2]
        <A "MtrlOutSpec">
        <L [0]>
    >
    <L [2]
        <A "ProcessingCtrlSpec">
        <L [1]
            <L [3]
                <A "pj00001">
                <L [0]>
                <L [0]>
            >
        >
    >
    <L [2]
        <A "StartMethod">
        <B 0>
    >
  >
>.
```

## 3.7 S16F11
➡️ Process Job ID 설정 메세지

```text
S16F11 W
<L [4]
  <U4 [1] 1>
  <A "pj00001">
  <B 0x01>
  <L [1]
    <L [2]
        <A "CSX 52078">
        <L [25]
            <U1 [1] 1>
            <U1 [1] 2>
            <U1 [1] 3>
            ...
            <U1 [1] 25>
        >
    >
  >
>.
```
