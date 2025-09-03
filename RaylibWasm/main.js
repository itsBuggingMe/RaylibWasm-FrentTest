import { dotnet } from './_framework/dotnet.js'

const runtime = await dotnet
    .withDiagnosticTracing(false)
    .create();

const config = runtime.getConfig();
const exports = await runtime.getAssemblyExports(config.mainAssemblyName);

dotnet.instance.Module['canvas'] = document.getElementById('canvas');

function mainLoop() {
    exports.RaylibWasm.Application.UpdateFrame();

    window.requestAnimationFrame(mainLoop);
}

await runtime.runMain();
window.requestAnimationFrame(mainLoop);