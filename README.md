# Hibiki

向かってくるウイルスを退治するゲームです

左右に動いて、ウイルスを掴んだり放したりできます

## ルール

- ウイルスは大中小の三種類です
- ウイルスを一段階大きいものに当てると割れます
- 割れたウイルスを一段階大きいものに当てると破壊されます
- 一段階小さいものを割れたウイルスに当てても破壊されます
- 破壊されたときに隣接する同色も破壊されます


# 設計メモ

外部→モデル:
- 保持機を左右に動かす Move (position) 
- 保持機のアームを伸ばしてウイルスを取る/置く Manipulate
- ウイルスが時間経過/手動でせり下がる Drop

モデル→モデル/外部:
- ウイルスの位置が変わる Change (ID, from, to)
- ウイルスが下のウイルスを吸収する Absorb (eaterID, eatenID)
- ウイルスが破壊される Break (ID)

盤面によっては Change→Absorb→Break→Change→… と連鎖する
