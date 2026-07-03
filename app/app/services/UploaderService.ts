import type {IApiResult} from "~~/types";
import type {UploaderTypes} from "~/models/Enums/UplaoderTypes";

export interface IUploadPayload {
    file: File
    fileType: UploaderTypes
    title?: string | null
    alt?: string | null
    description?: string | null
}

export default class UploaderService {
    constructor(private httpClient: LocalFetch) {
    }

    upload(payload: IUploadPayload): Promise<IApiResult> {
        const myForm = useUtils().generateAnyFormData(payload)
        return this.httpClient("Uploaders/Upload", {
            method: "POST",
            body: myForm,
        });
    }

    delete(guid: string): Promise<IApiResult> {
        return this.httpClient(`Uploaders/Delete/${guid}`, {
            method: "DELETE",
            params: {
                guid
            }
        });
    }

}
