---
id: DRAGON-003
title: "Tiến hóa Rồng 5 cấp theo EXP"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P1
milestone: M2
domain: dragon
dependencies: [DRAGON-002, ASSET-004]
source: "PRD US-3.1; evolution 1–5"
stack: [react, phaser-3, dotnet, postgresql]
---

# DRAGON-003 — Tiến hóa Rồng 5 cấp theo EXP

> **Spec-driven ticket** · Domain: **Dragon Lair & Evolution** · Priority: **P1** · Release: **M2** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là bé, tôi muốn thấy Rồng lớn dần khi tích lũy kinh nghiệm.

**Nguồn:** PRD US-3.1; evolution 1–5. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Evolution level chỉ từ server, trong khoảng 1..5; XP thresholds có version.
- Không bỏ qua milestone nếu nhận thưởng lớn; trigger sự kiện evolution once per level.
- Đổi sprite/animation tương ứng, có fallback nếu art chưa sẵn.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given EXP vượt threshold level 2
When feed commit
Then evolution_level=2 và animation phát một lần.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given max level 5
When cộng EXP
Then level vẫn 5, EXP xử lý theo cap policy.
```

## 4. Contract & Data Impact

- Proposed `evolution_rules` config thresholds + version; `dragon_states.evolution_level,exp_points`.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Nếu assets evolved missing, dùng sprite fallback nhưng backend level không đổi.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Threshold boundary tests, multi-level jump, retry evolution.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Rule table
- [ ] Domain evolution
- [ ] Scene animation
- [ ] Tests.

## 8. Dependencies & Boundaries

**Blocked by:** [DRAGON-002](../dragon/DRAGON-002-cho-rong-an-tieu-hao-food-cong-exp.md), [ASSET-004](../world-map/ASSET-004-sprite-rong-mau-sac-va-animation-states.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chốt XP thresholds và art assets 5 cấp (asset pack baby chưa đại diện đủ 5 cấp).

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-02 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
