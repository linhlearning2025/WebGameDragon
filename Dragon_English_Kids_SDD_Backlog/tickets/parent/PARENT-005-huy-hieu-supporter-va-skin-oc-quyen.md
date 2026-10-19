---
id: PARENT-005
title: "Huy hiệu supporter và skin độc quyền"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P2
milestone: M3
domain: parent
dependencies: [PARENT-004, DRAGON-005]
source: "PRD US-1.5"
stack: [react, phaser-3, dotnet, postgresql]
---

# PARENT-005 — Huy hiệu supporter và skin độc quyền

> **Spec-driven ticket** · Domain: **Parent Dashboard & Donation** · Priority: **P2** · Release: **M3** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là gia đình ủng hộ, tôi muốn thấy huy hiệu và cosmetic nhưng mọi bé vẫn học được như nhau.

**Nguồn:** PRD US-1.5. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Sau giao dịch verified, supporter badge hiển thị ở parent profile, cosmetic entitlement áp dụng scope được chốt.
- Không ảnh hưởng câu hỏi, điểm SRS, cơ chế unlock, rewards học tập.
- Idempotent payment event không cấp skin trùng; refund/revocation có policy.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given supporter verified
When render dashboard/lair
Then hiện badge và cosmetic hợp lệ.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given non-supporter
When chơi
Then không bị khóa kỹ năng hay nội dung Cambridge.
```

## 4. Contract & Data Impact

- Proposed `supporter_entitlements` table, entitlement event after verified donation.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Revoked entitlement không crash nếu skin đang equipped; fallback default.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Entitlement/functional parity tests, duplicate events, revocation.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Entitlement rules
- [ ] Badge UI
- [ ] Skin grant
- [ ] Tests.

## 8. Dependencies & Boundaries

**Blocked by:** [PARENT-004](../parent/PARENT-004-donate-qr-va-trang-thai-supporter.md), [DRAGON-005](../dragon/DRAGON-005-trang-phuc-rong-va-entitlement-cosmetics.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chốt cosmetics/skin id và scope family vs each child.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-02 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
