import { default as Axios } from "axios";
import { ISearchResult, IRole, ISecurityClaim } from "../../model";
import { GlobalConfig } from "../../common";
import { Store, StoreService } from "../../store";

// URL and endpoint constants
const BASE_URL = GlobalConfig.uri.services + "manage/role/";

const SEARCH_URL = BASE_URL + "search";
const CLONE_URL = BASE_URL + "clonerole";
const GET_URL = BASE_URL + "getrole";
const PUT_URL = BASE_URL + "updaterole";
const POST_URL = BASE_URL + "createrole";

export interface IGetRolePayload {
  allowedValuesPattern: string;
  availableClaims: ISecurityClaim[];
  existingRoles: IRole[];
  systemRoles: IRole[];
  role: IRole;
}

export class ManageRoleService extends StoreService {
  constructor(store: Store<{}>) {
    super(store);
  }

  cloneRole(id: string | number) {
    return this.exec<IGetRolePayload>(
      Axios.get(CLONE_URL, { params: { id } })
    ).then(value => this.processPayload(value));
  }

  getRole(id: string | number) {
    return this.exec<IGetRolePayload>(
      Axios.get(GET_URL, { params: { id } })
    ).then(value => this.processPayload(value));
  }

  search(page: number, pageSize: number) {
    return this.exec<ISearchResult<IRole>>(
      Axios.get(SEARCH_URL, { params: { page, pageSize } })
    ).then(value => this.processPayload(value));
  }

  createRole(options: IRole) {
    return this.exec<IRole>(Axios.post(POST_URL, options)).then(value =>
      this.processPayload(value)
    );
  }

  updateRole(options: IRole) {
    return this.exec<IRole>(Axios.put(PUT_URL, options)).then(value =>
      this.processPayload(value)
    );
  }

  updateRoleStatus(options: IRole) {
    const URL = BASE_URL + "updaterolestatus";
    return this.exec<IRole>(Axios.put(URL, options)).then(value =>
      this.processPayload(value)
    );
  }
}
