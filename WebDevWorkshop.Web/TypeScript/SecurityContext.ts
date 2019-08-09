import { UserManager, UserManagerSettings } from 'oidc-client';

export class SecurityContext {
    private userManager: UserManager;
    
    constructor(config: ISecurityContextConfig) {
        this.userManager = SecurityContext.getUserManager(config);
        this.userManager.events.addSilentRenewError(x => {
            if (config.onSignInFailure)
                config.onSignInFailure();
        })
    }

    getAccessToken() {
        return this.userManager.getUser().then(x => {
            if (!x || x.expired) {
                return this.userManager.signinSilent().then(x => x.access_token);
            }
            return x.access_token;
        })
    }

    private static getUserManager(config: ISecurityContextConfig) {
        let cfg: UserManagerSettings = {
            authority: config.authority,
            client_id: config.clientId,
            response_type: "id_token token",
            scope: "openid profile " + config.scopes.join(" "),
            silent_redirect_uri: new URL(config.redirectUri, window.location.href).href,
            automaticSilentRenew: true
        };
        return new UserManager(cfg);
    }
}

export interface ISecurityContextConfig {
    authority: string;
    clientId: string;
    scopes: string[];
    redirectUri: string;
    onSignInFailure: () => void;
}