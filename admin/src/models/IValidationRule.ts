export type ValidationResult = string | boolean

export type ValidationRule =
  | ValidationResult
  | PromiseLike<ValidationResult>
  | ((value: any) => ValidationResult)
  | ((value: any) => PromiseLike<ValidationResult>)

export interface IOrderedValidationRule {
  rules?: ValidationRule[] | null
  order?: number | null
}
