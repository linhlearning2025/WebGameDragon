---
id: GUEST-003
title: "Ngân hàng câu hỏi placement và ghi nhận câu trả lời"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M1
domain: guest
dependencies: [GUEST-001, CONTENT-001]
source: "PRD US-2.3; /guest/submit-test"
stack: [react, phaser-3, dotnet, postgresql]
---

# GUEST-003 — Ngân hàng câu hỏi placement và ghi nhận câu trả lời

> **Spec-driven ticket** · Domain: **Guest Trial & Placement** · Priority: **P0** · Release: **M1** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là hệ thống, tôi cần câu hỏi đầu vào có phân loại chủ đề/độ khó và không lặp vô lý.

**Nguồn:** PRD US-2.3; /guest/submit-test. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Mỗi item có questionId, vocabId, difficulty 1–3, type, choices và correct-answer server-side.
- Mỗi câu được ghi nhận tối đa một lần qua answerId/idempotency key.
- Không gửi correctAnswer trong payload client khi chưa nộp.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given câu 2 chưa trả lời
When submit answer
Then engine lưu kết quả và cấp câu tiếp.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given submit cùng answerId hai lần
When xử lý
Then không tăng số câu đã làm.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given altered choiceId không thuộc question
When submit
Then 400/422.
```

## 4. Contract & Data Impact

- Proposed `POST /api/v1/guest/{guestToken}/answers`; `GET .../next-question`.
- `temp_test_result` JSONB chứa state phiên có version và question history.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Network retry không đếm hai lần; không tin isCorrect từ client.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Idempotency, answer tampering, concurrency two tabs.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Question schema
- [ ] Answer endpoint
- [ ] Idempotency
- [ ] Contract tests.

## 8. Dependencies & Boundaries

**Blocked by:** [GUEST-001](../guest/GUEST-001-khoi-tao-phien-guest-va-chon-o-tuoi.md), [CONTENT-001](../content/CONTENT-001-vocabulary-catalog-metadata-schema-jsonb.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- PRD chỉ liệt kê `/guest/submit-test`: có thể gộp thành endpoint này nếu trả nextQuestion sau từng đáp án.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
