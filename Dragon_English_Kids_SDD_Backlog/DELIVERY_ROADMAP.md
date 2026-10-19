# DELIVERY_ROADMAP — Đề xuất triển khai theo dependency

Milestone là **phân kỳ đề xuất**, không phải deadline hay story point đã estimate. M0/M1/M2/M3 chỉ đánh dấu thứ tự ưu tiên; business sign-off có thể điều chỉnh.

## M0 — Foundation & security (7 tickets)

Nền tảng, schema, chuẩn từ vựng, privacy, authorization

- [CONTENT-001 — Vocabulary catalog + metadata schema JSONB](tickets/content/CONTENT-001-vocabulary-catalog-metadata-schema-jsonb.md) · **P0** · Dep: PLAT-003
- [PLAT-001 — Thiết lập monorepo và môi trường phát triển](tickets/platform/PLAT-001-thiet-lap-monorepo-va-moi-truong-phat-trien.md) · **P0** · Dep: —
- [PLAT-002 — Event Bridge React ↔ Phaser có kiểu dữ liệu](tickets/platform/PLAT-002-event-bridge-react-phaser-co-kieu-du-lieu.md) · **P0** · Dep: PLAT-001
- [PLAT-003 — PostgreSQL schema, migrations và domain constraints](tickets/platform/PLAT-003-postgresql-schema-migrations-va-domain-constraints.md) · **P0** · Dep: PLAT-001
- [PLAT-005 — Kiểm soát truy cập theo phụ huynh, trẻ và khách](tickets/platform/PLAT-005-kiem-soat-truy-cap-theo-phu-huynh-tre-va-khach.md) · **P0** · Dep: PLAT-003
- [QA-002 — Chính sách dữ liệu trẻ em, consent và bảo vệ thông tin](tickets/quality/QA-002-chinh-sach-du-lieu-tre-em-consent-va-bao-ve-thong-tin.md) · **P0** · Dep: PLAT-001
- [PLAT-004 — Correlation ID, logs, metrics và lỗi chuẩn](tickets/platform/PLAT-004-correlation-id-logs-metrics-va-loi-chuan.md) · **P1** · Dep: PLAT-001

## M1 — Guest-to-map MVP (20 tickets)

Đăng ký/guest/claim, profile, placement, map có level nodes động và dragon basic

- [ASSET-001 — Chuẩn asset manifest và preload theo phiên bản](tickets/world-map/ASSET-001-chuan-asset-manifest-va-preload-theo-phien-ban.md) · **P0** · Dep: PLAT-001
- [ASSET-002 — World Map responsive với camera và hit area](tickets/world-map/ASSET-002-world-map-responsive-voi-camera-va-hit-area.md) · **P0** · Dep: ASSET-001, PLAT-002
- [ASSET-003 — Rendering 4 node kỹ năng và trạng thái hoàn thành](tickets/world-map/ASSET-003-rendering-4-node-ky-nang-va-trang-thai-hoan-thanh.md) · **P0** · Dep: ASSET-002, MAP-002
- [AUTH-001 — Đăng ký/đăng nhập phụ huynh bằng email](tickets/identity/AUTH-001-ang-ky-ang-nhap-phu-huynh-bang-email.md) · **P0** · Dep: PLAT-003, PLAT-005
- [AUTH-003 — Tạo/chỉnh sửa hồ sơ trẻ tối đa 5](tickets/identity/AUTH-003-tao-chinh-sua-ho-so-tre-toi-a-5.md) · **P0** · Dep: AUTH-001
- [AUTH-004 — Chọn hồ sơ trẻ và cô lập session game](tickets/identity/AUTH-004-chon-ho-so-tre-va-co-lap-session-game.md) · **P0** · Dep: AUTH-003, PLAT-002
- [AUTH-005 — Parental Gate với PIN băm và xác thực](tickets/identity/AUTH-005-parental-gate-voi-pin-bam-va-xac-thuc.md) · **P0** · Dep: AUTH-001, PLAT-005
- [CONTENT-002 — Bộ dữ liệu chủ đề STARTERS/MOVERS và kiểm soát trùng](tickets/content/CONTENT-002-bo-du-lieu-chu-e-starters-movers-va-kiem-soat-trung.md) · **P0** · Dep: CONTENT-001
- [DRAGON-001 — Hang Rồng render state và điều hướng](tickets/dragon/DRAGON-001-hang-rong-render-state-va-ieu-huong.md) · **P0** · Dep: AUTH-003, ASSET-001
- [GUEST-001 — Khởi tạo phiên guest và chọn độ tuổi](tickets/guest/GUEST-001-khoi-tao-phien-guest-va-chon-o-tuoi.md) · **P0** · Dep: PLAT-003
- [GUEST-002 — Phaser Placement Scene “Đánh thức Trứng Rồng”](tickets/guest/GUEST-002-phaser-placement-scene-anh-thuc-trung-rong.md) · **P0** · Dep: GUEST-001, GUEST-003, PLAT-002
- [GUEST-003 — Ngân hàng câu hỏi placement và ghi nhận câu trả lời](tickets/guest/GUEST-003-ngan-hang-cau-hoi-placement-va-ghi-nhan-cau-tra-loi.md) · **P0** · Dep: GUEST-001, CONTENT-001
- [GUEST-004 — Thuật toán placement adaptive và phân tầng](tickets/guest/GUEST-004-thuat-toan-placement-adaptive-va-phan-tang.md) · **P0** · Dep: GUEST-003
- [GUEST-005 — Nở trứng, tạo Rồng guest và tùy chỉnh](tickets/guest/GUEST-005-no-trung-tao-rong-guest-va-tuy-chinh.md) · **P0** · Dep: GUEST-002, GUEST-004
- [GUEST-006 — Claim guest progress vào parent account một lần](tickets/guest/GUEST-006-claim-guest-progress-vao-parent-account-mot-lan.md) · **P0** · Dep: GUEST-005, AUTH-001, AUTH-003
- [MAP-001 — Domain model đảo, chủ đề và tuyến học](tickets/world-map/MAP-001-domain-model-ao-chu-e-va-tuyen-hoc.md) · **P0** · Dep: CONTENT-001, PLAT-003
- [MAP-002 — Quy tắc mở khóa và lưu stage progress](tickets/world-map/MAP-002-quy-tac-mo-khoa-va-luu-stage-progress.md) · **P0** · Dep: MAP-001, AUTH-003
- [MAP-003 — API world map theo profile + tier](tickets/world-map/MAP-003-api-world-map-theo-profile-tier.md) · **P0** · Dep: MAP-001, MAP-002, PLAT-005
- [MAP-004 — Chọn stage và chuyển Phaser mini-game](tickets/world-map/MAP-004-chon-stage-va-chuyen-phaser-mini-game.md) · **P0** · Dep: ASSET-003, MAP-003, PLAT-002
- [QA-001 — Đặc tả accessibility và UX an toàn cho trẻ 5–8](tickets/quality/QA-001-ac-ta-accessibility-va-ux-an-toan-cho-tre-58.md) · **P0** · Dep: PLAT-001

## M2 — Learning core & meta game (22 tickets)

4 mini-games, SRS, stage completion, reward, feeding, media pipeline

- [DRAGON-002 — Cho Rồng ăn: tiêu hao food, cộng EXP](tickets/dragon/DRAGON-002-cho-rong-an-tieu-hao-food-cong-exp.md) · **P0** · Dep: DRAGON-001, GAME-003
- [GAME-001 — Stage-data API trộn 70% mới / 30% ôn SRS](tickets/gameplay/GAME-001-stage-data-api-tron-70-moi-30-on-srs.md) · **P0** · Dep: CONTENT-002, SRS-002, MAP-002
- [GAME-002 — Ghi nhận kết quả stage idempotent và chống gian lận](tickets/gameplay/GAME-002-ghi-nhan-ket-qua-stage-idempotent-va-chong-gian-lan.md) · **P0** · Dep: GAME-001, MAP-002, SRS-001
- [GAME-003 — Quy tắc thưởng food, EXP, stars](tickets/gameplay/GAME-003-quy-tac-thuong-food-exp-stars.md) · **P0** · Dep: GAME-002
- [GAME-004 — Khung mini-game chung, trạng thái và phản hồi tích cực](tickets/gameplay/GAME-004-khung-mini-game-chung-trang-thai-va-phan-hoi-tich-cuc.md) · **P0** · Dep: PLAT-002, GAME-001
- [GAME-005 — Listening — Rồng đớp mây đúng hình](tickets/gameplay/GAME-005-listening-rong-op-may-ung-hinh.md) · **P0** · Dep: GAME-004
- [GAME-006 — Reading — chọn từ phá đá](tickets/gameplay/GAME-006-reading-chon-tu-pha-a.md) · **P0** · Dep: GAME-004
- [GAME-007 — Spelling — ghép phonics xây cầu](tickets/gameplay/GAME-007-spelling-ghep-phonics-xay-cau.md) · **P0** · Dep: GAME-004, CONTENT-001
- [GAME-008 — Speaking — quyền micro, capture và fallback](tickets/gameplay/GAME-008-speaking-quyen-micro-capture-va-fallback.md) · **P0** · Dep: GAME-004
- [GAME-009 — Speaking — fuzzy matching và 2-strike effort-pass](tickets/gameplay/GAME-009-speaking-fuzzy-matching-va-2-strike-effort-pass.md) · **P0** · Dep: GAME-008, GAME-002
- [GAME-010 — Stage summary, replay và quay lại World Map](tickets/gameplay/GAME-010-stage-summary-replay-va-quay-lai-world-map.md) · **P0** · Dep: GAME-002, GAME-003, MAP-004
- [PARENT-003 — Giới hạn thời gian chơi mỗi ngày có server policy](tickets/parent/PARENT-003-gioi-han-thoi-gian-choi-moi-ngay-co-server-policy.md) · **P0** · Dep: AUTH-003, PLAT-005
- [QA-003 — Contract tests và E2E 4 mini-game qua backend](tickets/quality/QA-003-contract-tests-va-e2e-4-mini-game-qua-backend.md) · **P0** · Dep: GAME-005, GAME-006, GAME-007, GAME-009, MAP-003
- [SRS-001 — Domain SRS mastery, streak và lịch ôn](tickets/srs/SRS-001-domain-srs-mastery-streak-va-lich-on.md) · **P0** · Dep: CONTENT-001, AUTH-003
- [SRS-002 — Query từ đến hạn và phân phối pool theo tier](tickets/srs/SRS-002-query-tu-en-han-va-phan-phoi-pool-theo-tier.md) · **P0** · Dep: SRS-001, CONTENT-002
- [ASSET-004 — Sprite Rồng màu sắc và animation states](tickets/world-map/ASSET-004-sprite-rong-mau-sac-va-animation-states.md) · **P1** · Dep: ASSET-001, DRAGON-001
- [AUTH-002 — Đăng nhập Google OAuth phụ huynh](tickets/identity/AUTH-002-ang-nhap-google-oauth-phu-huynh.md) · **P1** · Dep: AUTH-001
- [CONTENT-003 — Console Tool C# sinh nội dung AI có kiểm duyệt](tickets/content/CONTENT-003-console-tool-c-sinh-noi-dung-ai-co-kiem-duyet.md) · **P1** · Dep: CONTENT-001, CONTENT-002
- [CONTENT-004 — TTS âm thanh thường/chậm và upload Cloudflare R2](tickets/content/CONTENT-004-tts-am-thanh-thuong-cham-va-upload-cloudflare-r2.md) · **P1** · Dep: CONTENT-003
- [CONTENT-005 — Quy trình review/publish học liệu và asset](tickets/content/CONTENT-005-quy-trinh-review-publish-hoc-lieu-va-asset.md) · **P1** · Dep: CONTENT-004
- [DRAGON-003 — Tiến hóa Rồng 5 cấp theo EXP](tickets/dragon/DRAGON-003-tien-hoa-rong-5-cap-theo-exp.md) · **P1** · Dep: DRAGON-002, ASSET-004
- [QA-004 — Hiệu năng âm thanh, preload và R2 CDN](tickets/quality/QA-004-hieu-nang-am-thanh-preload-va-r2-cdn.md) · **P1** · Dep: CONTENT-004, GAME-005, ASSET-001

## M3 — Parent & launch (9 tickets)

Reports, donation/cosmetics, accessibility/polish và release

- [AUTH-007 — Xóa tài khoản/hồ sơ và xuất dữ liệu cơ bản](tickets/identity/AUTH-007-xoa-tai-khoan-ho-so-va-xuat-du-lieu-co-ban.md) · **P1** · Dep: AUTH-003, PLAT-005
- [PARENT-001 — API báo cáo tiến trình từ vựng và thời gian](tickets/parent/PARENT-001-api-bao-cao-tien-trinh-tu-vung-va-thoi-gian.md) · **P1** · Dep: AUTH-003, SRS-001, GAME-002
- [PARENT-002 — Parent Dashboard UI có lọc theo bé](tickets/parent/PARENT-002-parent-dashboard-ui-co-loc-theo-be.md) · **P1** · Dep: PARENT-001, AUTH-005
- [QA-005 — Phát hành, backup/restore, feature flags và runbook](tickets/quality/QA-005-phat-hanh-backup-restore-feature-flags-va-runbook.md) · **P1** · Dep: PLAT-004, QA-003
- [AUTH-006 — Parental Gate thay thế bằng câu hỏi toán](tickets/identity/AUTH-006-parental-gate-thay-the-bang-cau-hoi-toan.md) · **P2** · Dep: AUTH-005
- [DRAGON-004 — Tương tác vuốt ve Rồng tăng Mood](tickets/dragon/DRAGON-004-tuong-tac-vuot-ve-rong-tang-mood.md) · **P2** · Dep: DRAGON-001
- [DRAGON-005 — Trang phục Rồng và entitlement cosmetics](tickets/dragon/DRAGON-005-trang-phuc-rong-va-entitlement-cosmetics.md) · **P2** · Dep: DRAGON-001, PARENT-004
- [PARENT-004 — Donate QR và trạng thái supporter](tickets/parent/PARENT-004-donate-qr-va-trang-thai-supporter.md) · **P2** · Dep: AUTH-005, AUTH-001
- [PARENT-005 — Huy hiệu supporter và skin độc quyền](tickets/parent/PARENT-005-huy-hieu-supporter-va-skin-oc-quyen.md) · **P2** · Dep: PARENT-004, DRAGON-005

## Suggested implementation order (topological, not per-team sprint allocation)

`PLAT-001` → `PLAT-002` → `PLAT-003` → `QA-002` → `PLAT-004` → `ASSET-001` → `QA-001` → `CONTENT-001` → `PLAT-005` → `ASSET-002` → `GUEST-001` → `AUTH-001` → `CONTENT-002` → `GUEST-003` → `MAP-001` → `AUTH-003` → `AUTH-005` → `GUEST-002` → `GUEST-004` → `AUTH-002` → `CONTENT-003` → `AUTH-004` → `DRAGON-001` → `GUEST-005` → `MAP-002` → `PARENT-003` → `SRS-001` → `CONTENT-004` → `AUTH-007` → `AUTH-006` → `PARENT-004` → `ASSET-003` → `GUEST-006` → `MAP-003` → `SRS-002` → `ASSET-004` → `CONTENT-005` → `DRAGON-004` → `DRAGON-005` → `MAP-004` → `GAME-001` → `PARENT-005` → `GAME-002` → `GAME-004` → `GAME-003` → `GAME-005` → `GAME-006` → `GAME-007` → `GAME-008` → `PARENT-001` → `DRAGON-002` → `GAME-009` → `GAME-010` → `QA-004` → `PARENT-002` → `QA-003` → `DRAGON-003` → `QA-005`
