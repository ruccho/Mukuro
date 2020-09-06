# Mukuro
GUI event scripting editor / playing environment in Unity

Mukuroは編集効率や拡張性を重視したイベントスクリプトエディタです。

![image](https://user-images.githubusercontent.com/16096562/92329301-95b09500-f0a1-11ea-9cf1-f62fcd6b3ddb.png)

## Requirements

 - Unity 2019.3+

## Getting started

Mukuroは複数のパッケージに分割されており、用途に応じて必要なものを選択できる設計になっています。
公開しているパッケージは以下の通りです。

| パッケージ名 | UPMのパッケージ名              | 説明                                                     | 依存パッケージ |
|--------------|--------------------------------|----------------------------------------------------------|----------------|
| Core         | io.github.ruccho.mukuro.core   | Mukuroのエディタと再生環境、基本的なコマンドを含みます。 | なし           |
| Dialog       | io.github.ruccho.mukuro.dialog | メッセージと選択肢の表示に関するコマンドを含みます。     | Core           |


### Core パッケージのインストール
MukuroのパッケージはUnity Package Manager (UPM) 形式で公開しています。 Core パッケージをインストールするには、Packages/manifest.jsonを編集し、dependenciesに以下の2項目を追記します：
```json:manifest.json

  "dependencies": {
      ...
    "io.github.ruccho.mukuro.core": "https://github.com/ruccho/Mukuro.git?path=/Packages/io.github.ruccho.mukuro.core",
    "io.github.ruccho.exposedunityevent": "https://github.com/ruccho/ExposedUnityEvent.git?path=/Packages/io.github.ruccho.exposedunityevent"
    ...
  }

```

### イベントスクリプトの作成
1. Projectビューで右クリック > Create > Mukuro > CommandScriptでイベントを記述するアセットを作成できます。
2. 作成したアセットのInspectorからイベントスクリプトエディタウィンドウを起動できます。

![image](https://user-images.githubusercontent.com/16096562/92329330-bf69bc00-f0a1-11ea-9623-cbc496050f87.png)

3. 左ペインからコマンドを選択して中央のペインにコマンドを追加できます。
   - 追加されたコマンドは上から順に実行されます。
   - 追加したコマンドはドラッグで移動できます。
   - 追加したコマンドは選択してDeleteキーで削除できます。
4. 追加したコマンドを選択すると、右ペインにコマンドの詳細な設定が表示されます。

### イベントの再生
1. シーン上の適当なGameObjectにEventScriptPlayerをアタッチします。
2. 以下のスクリプトを作成し適当なGameObjectにアタッチします：
```csharp
using UnityEngine;

public class MukuroEventPlayerSample : MonoBehaviour
{
    [SerializeField] private EventScriptAsset script = default;
    [SerializeField] private EventScriptPlayer player = default;
    
    public void Play()
    {
        player.Play(script, gameObject.scene);
    }
}
```
3. Play()を呼ぶとイベントが再生されます。