---
id: DRAGON-005
title: "Trang phục Rồng và entitlement cosmetics"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P2
milestone: M3
domain: dragon
dependencies: [DRAGON-001, PARENT-004]
source: "PRD US-3.1, US-1.5"
stack: [react, phaser-3, dotnet, postgresql]
---

# DRAGON-005 — Trang phục Rồng và entitlement cosmetics

> **Spec-driven ticket** · Domain: **Dragon Lair & Evolution** · Priority: **P2** · Release: **M3** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là bé, tôi muốn đổi phụ kiện Rồng được mở khóa hợp lệ.

**Nguồn:** PRD US-3.1, US-1.5. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Inventory cosmetics thuộc child hoặc family entitlement theo rule được chốt.
- Trang bị skin kiểm tra ownership; chỉ 1 skin mỗi slot; không biến cosmetics thành lợi thế học tập.
- Supporter skin khi entitlement hết hạn/thay đổi phải có policy rõ.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given child sở hữu hat
When equip
Then equipped_skins cập nhật.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given chưa sở hữu skin
When equip
Then 403 business error.
```

## 4. Contract & Data Impact

- Proposed `POST /api/v1/game/{childId}/dragon/equip`; JSONB `equipped_skins` schema.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Missing sprite skin: hiển thị default, entitlement vẫn giữ.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Entitlement, conflicts, visual fallback and ownership.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Skin catalog
- [ ] Equip service
- [ ] Lair selection UI
- [ ] Tests.

## 8. Dependencies & Boundaries

**Blocked by:** [DRAGON-001](../dragon/DRAGON-001-hang-rong-render-state-va-ieu-huong.md), [PARENT-004](../parent/PARENT-004-donate-qr-va-trang-thai-supporter.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chốt cosmetic slots và donations entitlement model.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-02 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
