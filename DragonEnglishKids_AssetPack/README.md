# Dragon English Kids — Bộ asset World Map + Level Select

**Nội dung:** 135 PNG trong đó 132 sprites/nền được vẽ riêng bằng hình học/vector (bao gồm 6 Rồng con và 20 animation frames), 1 bản đồ concept tham khảo; `manifest.json`; ảnh preview; hợp đồng dữ liệu và ví dụ React + Phaser 3 + .NET.

> ⚠️ PNG là tài nguyên hình ảnh **tĩnh**. Chúng không tự gọi API. Phaser/React tải JSON backend và dựa vào `theme`, `skin`, `state`, `stars`, `exp`, `unlocked` để quyết định sprite nào được hiển thị, thêm chữ và tạo tương tác. Các số level, sao, chữ và EXP nên được render bằng code chứ không vẽ cố định lên nền.

## 1. Sử dụng nhanh

1. Copy toàn bộ `public/assets/game/` vào `public/assets/game/` của dự án React (Vite hoặc tương đương).
2. Cài đặt `npm install phaser`.
3. Copy `integration/phaser/WorldMapScene.js` và `integration/react/DragonWorldMap.jsx` vào `src/`, điều chỉnh đường dẫn import.
4. Bổ sung endpoint **mới** `GET /api/v1/game/{childId}/world-map` bên .NET, theo JSON mẫu ở `public/mock/world-map.json`. Endpoint này là đề xuất bổ sung cho PRD, không phải endpoint đã có sẵn.
5. Render `<DragonWorldMap childId={child.id} onStageSelected={handleStageSelected} />` ở component React.
6. Khi bé chọn một kỹ năng, `handleStageSelected` điều hướng sang mini-game và gọi endpoint trong PRD `/api/v1/game/{childId}/stage-data` (điều chỉnh contract cho stage cụ thể).

### Thử demo KHÔNG cần Phaser hoặc backend

Tại thư mục `public/` của bản ZIP:

```bash
python -m http.server 5173
```

Mở `http://localhost:5173/demo.html`. Bấm các đảo và nút **Đổi trạng thái tiến trình** để xem chính cùng bộ PNG cập nhật khi JSON thay đổi. Để dùng API thực, có thể mở `demo.html?api=/api/v1/game/<childId>/world-map` sau khi proxy API được cấu hình. Demo dựa trên Canvas 2D chỉ để kiểm tra asset, **không phải ứng dụng Phaser chính thức**.

## 2. Cấu trúc thư mục

```text
DragonEnglishKids_AssetPack/
├── public/
│   ├── assets/game/
│   │   ├── world-map/
│   │   │   ├── backgrounds/     # sky_day, sky_sunset, distant mountains, clouds, map concept
│   │   │   ├── islands/         # 12 đảo (lair, forest, food, crystal, family...)
│   │   │   ├── paths/           # bridges, paths, stepping stones, portals
│   │   │   └── decorations/     # trees, bushes, school, rocks, waterfall ...
│   │   ├── level-nodes/
│   │   │   ├── listening/       # node, icon, node_highlighted
│   │   │   ├── reading/
│   │   │   ├── spelling/
│   │   │   ├── speaking/
│   │   │   └── states/          # available, current, completed, locked, review, stars
│   │   ├── dragon/
│   │   │   ├── baby/            # red, blue, green, purple, pink, yellow
│   │   │   └── animations/      # idle/walk/fly/eat/happy × 4 frames (red)
│   │   ├── ui/
│   │   │   ├── buttons/
│   │   │   ├── panels/
│   │   │   ├── icons/
│   │   │   └── progress/
│   │   └── manifest.json       # URL / kích thước toàn bộ PNG
│   ├── mock/world-map.json    # Ví dụ backend trả dữ liệu động
│   └── demo.html              # Demo tự chạy bằng HTTP server
├── integration/
│   ├── phaser/WorldMapScene.js
│   ├── react/DragonWorldMap.jsx
│   └── backend-dotnet/WorldMapController.cs
├── preview_world_map.png
├── preview_asset_samples.png
├── concept_asset_overview_reference.png
└── README.md
```

## 3. Dữ liệu backend điều khiển asset

Các trường quan trọng trong `public/mock/world-map.json`:

| Backend field | Tác động trên Phaser |
|---|---|
| `world.background` | `sky_day.png` hoặc `sky_sunset.png` |
| `world.islands[].theme` | `world-map/islands/{theme}_island.png` |
| `world.islands[].x`, `.y` | Vị trí đảo tính bằng hệ tọa độ 1600 × 900 |
| `world.islands[].unlocked` | Điều khiển trạng thái khóa, đổi màu, ngăn mở màn |
| `world.islands[].stages[].skill` | `listening`, `reading`, `spelling`, `speaking` |
| `world.islands[].stages[].state` | `available`, `current`, `completed`, `locked`, `review` |
| `world.islands[].stages[].stars` | Vẽ sao 0..3 bằng Phaser text/icon |
| `dragon.skin` | Chọn file `dragon/baby/baby_{skin}.png` |
| `dragon.exp`, `dragon.expToNextLevel` | Vẽ/crop thanh tiến trình EXP |
| `progress.totalStars`, `maxStars` | Cập nhật tổng sao ở HUD |
| `world.connections` | Vẽ đường nối giữa các đảo |

**Quy tắc:** backend quyết định quyền mở khóa và cập nhật SRS. Đừng dựa vào `state` từ trình duyệt khi tính thưởng. Kiểm tra chủ sở hữu `childId` cho mọi API; validate stage completion phía server. Với game cho trẻ em, giới hạn thu thập dữ liệu, bảo vệ guest token, và đảm bảo parental gate ở server.

## 4. Responsive

Sprite có nền trong suốt (trừ 2 sky backgrounds và concept map). Logic map quy ước 1600×900, `Phaser.Scale.FIT` bảo toàn tỷ lệ khung ảnh trên desktop/tablet. Ở mobile màn dọc, 1600×900 sẽ quá nhỏ; nên chọn **camera zoom + pan/swipe** hoặc định nghĩa riêng vị trí đảo từ backend cho portrait thay vì ép toàn bộ bản đồ vào màn hình hẹp.

Trên asset nền trong suốt, vị trí đối tượng thường căn bằng `setOrigin(.5)`; khi hiển thị island file gốc 512×340 thì có thể `setDisplaySize(310,206)` như sample. `manifest.json` chứa kích thước PNG. Asset buttons và panels là hình raster, nên với UI responsive nâng cao hãy xây nine-slice hoặc render panel dạng vector/DOM cho vùng thay đổi kích thước lớn.

## 5. Lưu ý về chất lượng và giới hạn

- Toàn bộ PNG sprite modular là **bộ cartoon vector-styled đồng bộ**, khác phong cách tô vẽ chi tiết của hình concept trước đó; concept chỉ để tham khảo thị giác.
- Animation hiện gồm **20 frame vẽ đơn giản** cho Rồng đỏ; nếu muốn hoạt họa studio-quality cần mở rộng sprite sheet/Spine hoặc Rive.
- Không có mã backend .NET hoàn chỉnh hoặc dữ liệu 750 từ vựng. Controller là skeleton chỉ mô tả API contract; bạn cần kết nối EF Core + PostgreSQL và logic unlock/SRS.
- Những asset về level/select được thiết kế để **bên React/Phaser hiển thị dữ liệu động**, không thể nhận backend trực tiếp từ PNG.

## 6. Danh mục học tập gợi ý

Gán `theme` theo các chủ đề trong PRD: `forest` → Animals, `food` → Food, `crystal` → Colors, `family` → Family, `school` → School, `toys` → Toys, `nature` → Nature, `castle` → Movers. Mỗi đảo có tối đa 4 stage Listening, Reading, Spelling và Speaking; mỗi stage có 3 sao. Bộ này là mẫu UI, không tự chứng nhận phạm vi từ vựng Cambridge.
