import Vue from "vue";
import Component from "vue-class-component";
import { required } from "vuelidate/lib/validators";
import { ISecurityClaim, IRoleSecurityClaim, IRole } from "../../model";
import { IVueSelectOption } from "../../plugins";
import { Vuelidate } from "vuelidate";

// array is populated during component creation
let claimValidators: Map<string, RegExp> = null;

// dynamic vuelidate claim. performs a lookup to get associated regex
const validClaimValue = (value, vm) => {
  let v = claimValidators.get(vm.securityClaimId);

  if (v === undefined) {
    return false;
  } else {
    // console.log(`Evaluating ${value} against ${v}`);
    return v.test(value);
  }
};

const props = {
  allowedValuesPattern: { required: true },
  availableClaims: { required: true },
  selectedClaims: {
    default: []
  }
};

const validations = {
  selectedClaims: {
    $each: {
      value: {
        required,
        validClaimValue
      }
    }
  }
};

@Component({
  components: {},
  props: props,
  template: require("./claims-editor.html"),
  validations: validations
})
export class SecurityClaimsEditor extends Vue {
  $v: Vuelidate<any>;
  allowedValuesPattern: string = this.allowedValuesPattern;
  availableClaims: ISecurityClaim[] = this.availableClaims;
  claimOptionDefaults: IVueSelectOption[] = [];
  selectedClaims: IRoleSecurityClaim[] = this.selectedClaims || [];

  created() {
    this.createClaimValidations();
    let claimOptionDefaults = (this.claimOptionDefaults = this.createClaimOptionDefaults());
    this.selectedClaims.forEach(o =>
      this.updateClaim(o, this.claimOptionDefaults)
    );
  }

  useDefaultEditor(claim: IRoleSecurityClaim) {
    if (!claim) return false;

    let match = this.availableClaims.find(o => {
      return (
        o.securityClaimId === claim.securityClaimId &&
        o.validationPattern === this.allowedValuesPattern
      );
    });

    return match !== undefined;
  }

  public get unusedClaims() {
    let filter = (claim: ISecurityClaim) => {
      return (
        this.selectedClaims.find(
          o => claim.securityClaimId === o.securityClaimId
        ) === undefined
      );
    };

    return this.availableClaims.filter(filter);
  }

  public removeClaim(claim: IRoleSecurityClaim) {
    let index = this.selectedClaims.findIndex(
      o => o.securityClaimId === claim.securityClaimId
    );

    if (claim["unwatch"] && typeof claim["unwatch"] === "function")
      claim.unwatch();

    this.selectedClaims.splice(index, 1);
    this.onClaimValueChange();
  }

  public addClaim(value: ISecurityClaim) {
    var claim: IRoleSecurityClaim = {
      inherited: false,
      securityClaim: value,
      securityClaimId: value.securityClaimId,
      value: null,
      values: []
    };

    this.selectedClaims.push(claim);

    claim.unwatch = this.setClaimWatch(claim);
    this.$emit("change", [false]);
  }

  private updateClaim(
    claim: IRoleSecurityClaim,
    claimOptionDefaults: IVueSelectOption[]
  ) {
    let match = this.availableClaims.find(
      o => o.securityClaimId === claim.securityClaimId
    );

    claim.securityClaim = match;

    if (match && match.validationPattern === this.allowedValuesPattern) {
      let values = Array.from(claim.value);

      let value = values.map(o => claimOptionDefaults.find(c => c.id === o));

      Vue.set(claim, "values", value);
      claim.unwatch = this.setClaimWatch(claim);
    }
  }

  private setClaimWatch(claim: IRoleSecurityClaim) {
    let values = [
      this.$watch(() => claim.values, this.onClaimChange(claim)),
      this.$watch(() => claim.value, this.onClaimValueChange),
      () => {
        this.$v.selectedClaims.$each;
      }
    ];

    return () => values.forEach(o => o());
  }

  private createClaimOptionDefaults(): IVueSelectOption[] {
    return [
      { id: "C", label: this.$t("dict.create").toString() },
      { id: "R", label: this.$t("dict.read").toString() },
      { id: "U", label: this.$t("dict.update").toString() },
      { id: "D", label: this.$t("dict.delete").toString() },
      { id: "X", label: this.$t("dict.deny").toString() }
    ];
  }

  private createClaimValidations() {
    var validations = new Map<string, RegExp>();
    this.availableClaims.forEach(o => {
      validations.set(o.securityClaimId, new RegExp(o.validationPattern));
    });
    claimValidators = validations;
  }

  private onClaimChange(
    claim: IRoleSecurityClaim
  ): (this: this, n: any[], o: any[]) => void {
    return (newValue: IVueSelectOption[]) => {
      let deny = newValue.find(o => o.id === "X");
      if (deny && newValue.length === 1) {
        claim.value = deny.id;
      } else if (deny && newValue.length > 1) {
        this.$nextTick().then(() => {
          claim.values = [deny];
        });
      } else {
        claim.value = newValue.map(o => o.id).join("");
      }
    };
  }

  private onClaimValueChange() {
    let valid = !this.$v.selectedClaims.$error;
    this.$emit("change", [valid]);
  }

  public yesOrNo(value: boolean) {
    return value ? this.$t("dict.yes") : this.$t("dict.no");
  }
}
