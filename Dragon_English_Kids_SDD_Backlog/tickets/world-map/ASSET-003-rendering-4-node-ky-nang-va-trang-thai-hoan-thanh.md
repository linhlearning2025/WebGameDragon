---
id: ASSET-003
title: "Rendering 4 node kỹ năng và trạng thái hoàn thành"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M1
domain: world-map
dependencies: [ASSET-002, MAP-002]
source: "PRD 4 mini-games, tiến trình và asset paths"
stack: [react, phaser-3, dotnet, postgresql]
---

# ASSET-003 — Rendering 4 node kỹ năng và trạng thái hoàn thành

> **Spec-driven ticket** · Domain: **World Map & Assets** · Priority: **P0** · Release: **M1** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là bé, tôi nhìn thấy kỹ năng Nghe/Đọc/Ghép/Nói và level nào đang mở.

**Nguồn:** PRD 4 mini-games, tiến trình và asset paths. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Các node có loại listening/reading/spelling/speaking và state locked/available/current/completed.
- Tổng sao 0–3 từ backend; locked không interactive; completed có sao, current có glow animation.
- Overlay dynamic bằng sprite/text độc lập, không bake số thứ tự và sao vào hình nền.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given stage locked
When tap
Then không start stage và có gentle hint.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given stage completed 2 stars
When map render
Then đúng 2 gold stars.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given backend thay state after completion
When event refresh
Then node cập nhật không rebuild toàn scene.
```

## 4. Contract & Data Impact

- Stage node DTO: `stageId,skill,state,stars,x,y,islandId`; assets node/state keys.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Invalid stars/state: validator clamp/skip với error telemetry, không unlock tùy tiện.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] State matrix visual and click tests, dynamic response updates.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Node renderer
- [ ] Interactions
- [ ] Star icons
- [ ] Refresh tests.

## 8. Dependencies & Boundaries

**Blocked by:** [ASSET-002](../world-map/ASSET-002-world-map-responsive-voi-camera-va-hit-area.md), [MAP-002](../world-map/MAP-002-quy-tac-mo-khoa-va-luu-stage-progress.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chốt rule hiển thị current khi nhiều stage khả dụng.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
