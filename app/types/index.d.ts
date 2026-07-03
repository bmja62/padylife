import type { $Fetch } from "ofetch";
import type { Option } from "vue3-select-component";
import type { IApiProvider } from "@/models/IApiProvider";

export {
  IApiError,
  IApiResult,
  ApiResultStatusCode,
  IGlobalGridRequest,
  IGlobalGridResult,
  InputTypeHTMLAttribute,
  InputMode,
  LocalFetch,
  ISelectOptions,
  ITableHeaders,
  UserRole,
};

declare module "#app" {
  interface NuxtApp {
    $api: IApiProvider;
  }
}

declare global {
  type UserRole = "specialist" | "user";

  interface ITableHeaders<T> {
    label: string;
    key: keyof T | "index" | "actions";
    formatter?: (value: T[keyof T], label: string, item: T) => string; // Optional formatter
  }
  type ITableItems<T> = T;

  type ISelectOptions<T> = Option<T>;
  interface Date {
    customToISOString(): string;
  }
  interface IApiError<TError = string> {
    isSuccess: boolean;
    statusCode: ApiResultStatusCode;
    errors: TError;
  }
  interface IApiResult<TData = void> {
    isSuccess: boolean;
    statusCode: ApiResultStatusCode;
    message?: string;
    errors?: string;
    data: TData;
  }
  type LocalFetch = $Fetch;

  interface AxiosInstance extends Axios {
    <T = unknown, R = AxiosResponse<T>, D = unknown>(
      config: AxiosRequestConfig<D>
    ): Promise<R>;
    <T = unknown, R = AxiosResponse<T>, D = unknown>(
      url: string,
      config?: AxiosRequestConfig<D>
    ): Promise<R>;

    defaults: Omit<AxiosDefaults, "headers"> & {
      headers: HeadersDefaults & {
        [key: string]: AxiosHeaderValue;
      };
    };
  }

  interface IGlobalGridRequest {
    pageNumber: number;
    count: number;
    search?:string
  }

  interface IGlobalGridResult<T = void> {
    data: T;
    total: number;
  }

  enum ApiResultStatusCode {
    success = 0,
    serverEror = 1,
    badRequest = 2,
    notFound = 3,
    listEmpty = 4,
    loginError = 5,
    unAuthorized = 6,
    ExpiredCode = "ExpiredCode",
  }
  type InputMode =
    | "email"
    | "search"
    | "tel"
    | "text"
    | "url"
    | "none"
    | "numeric"
    | "decimal";
  type InputTypeHTMLAttribute =
    | "button"
    | "checkbox"
    | "color"
    | "date"
    | "datetime-local"
    | "email"
    | "file"
    | "hidden"
    | "image"
    | "month"
    | "number"
    | "password"
    | "radio"
    | "range"
    | "reset"
    | "search"
    | "submit"
    | "tel"
    | "text"
    | "time"
    | "url"
    | "week";
}
