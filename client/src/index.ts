import { serve } from "bun";
import index from "./index.html";

const server = serve({
  routes: {
    // every url shows our React app
    "/*": index,
  },

  development: process.env.NODE_ENV !== "production" && {
    hmr: true,
    console: true,
  },
});

console.log(`Server running at ${server.url}`);
